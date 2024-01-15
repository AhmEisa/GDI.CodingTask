using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
#nullable disable

namespace TaxiBooking.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
            _httpContextAccessor = httpContextAccessor;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var token = await AuthenticateAsync(UserId, Password);
                if (!string.IsNullOrEmpty(token))
                {
                    await SetTokenCookie(token);
                    return RedirectToPage("/Index");
                }
            }
            return Page();
        }
        private async Task SetTokenCookie(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userId = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId), };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            });
            var iaAuth = User.Identity.IsAuthenticated;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(1),
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);
        }
        private async Task<string> AuthenticateAsync(string username, string password)
        {
            var apiUrl = $"/api/Auth/login";

            var credentials = new
            {
                username,
                password
            };

            try
            {
                var json = JsonSerializer.Serialize(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    var token = JsonSerializer.Deserialize<TokenResponse>(tokenResponse)?.token;
                    return token;
                }

                ModelState.AddModelError(string.Empty, "Invalid response from the authentication API");
                return string.Empty;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return string.Empty;
            }
        }

        private class TokenResponse
        {
            public string token { get; set; }
        }

        public async Task<IActionResult> SomeActionRequiringAuthentication()
        {
            // Retrieve the token from the cookie
            var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

            // Ensure the token is not null or empty
            if (string.IsNullOrEmpty(token))
            {
                // Redirect to the login page or handle unauthorized access
                return RedirectToPage("/Account/Login");
            }

            // Set the token in the request header
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Make a request to the API using the HttpClient with the added token
            var apiResponse = await _httpClient.GetAsync("your_api_url/some-action");

            if (apiResponse.IsSuccessStatusCode)
            {
                // Handle successful API response
                var responseData = await apiResponse.Content.ReadAsStringAsync();
                // Process the response data as needed
                return Content(responseData);
            }
            else
            {
                // Handle API error response
                return Content($"API Error: {apiResponse.StatusCode}");
            }
        }

    }
}
