using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace TaxiBooking.Web.Pages.Cars
{
    public class DetailsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public DetailsModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        public CarViewModel Car { get; set; }

        public async Task<IActionResult> OnGetAsync(string carNo)
        {
            var apiUrl = $"{_configuration["ApiBaseUrl"]}/api/cars/{carNo}"; // Replace with your actual API endpoint
            var token = HttpContext.Request.Cookies["AuthToken"]; // Retrieve the token from the cookie

            if (string.IsNullOrEmpty(token))
            {
                // Redirect to the login page or handle unauthorized access
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var carJson = await response.Content.ReadAsStringAsync();
                Car = JsonSerializer.Deserialize<CarViewModel>(carJson);
                return Page();
            }
            else
            {
                // Handle API error response
                return Content($"API Error: {response.StatusCode}");
            }
        }

    }
}
