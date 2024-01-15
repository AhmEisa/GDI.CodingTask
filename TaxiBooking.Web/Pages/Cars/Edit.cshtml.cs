using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace TaxiBooking.Web.Pages.Cars
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public EditModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            var apiUrl = $"{_configuration["ApiBaseUrl"]}/api/cars/{Car.Id}"; // Replace with your actual API endpoint
            var token = HttpContext.Request.Cookies["AuthToken"]; // Retrieve the token from the cookie

            if (string.IsNullOrEmpty(token))
            {
                // Redirect to the login page or handle unauthorized access
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var carJson = JsonSerializer.Serialize(Car);
            var content = new StringContent(carJson, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./ListCars");
            }
            else
            {
                // Handle API error response
                return Content($"API Error: {response.StatusCode}");
            }
        }

    }
}
