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
                    var token = JsonSerializer.Deserialize<TokenResponse>(tokenResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.Token;
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
            public string Token { get; set; }
        }

    }
}
