using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
#nullable disable

namespace TaxiBooking.Web.Pages.Cars
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public IndexModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        public PaginatedCars PaginatedCars { get; set; }

        [TempData]
        public string Message { get; set; }
        public async Task<IActionResult> OnGetAsync(int page = 1, int pageSize = 10)
        {
            var apiUrl = $"/api/car?page={page}&pageSize={pageSize}";
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
                var carsJson = await response.Content.ReadAsStringAsync();
                PaginatedCars = JsonSerializer.Deserialize<PaginatedCars>(carsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Page();
            }
            else
            {
                // Handle API error response
                return Content($"API Error: {response.StatusCode}");
            }
        }
        public async Task<IActionResult> OnDeleteAsync(long id)
        {
            var apiUrl = $"/api/cars/{id}"; // Replace with your actual API endpoint
            var token = HttpContext.Request.Cookies["AuthToken"]; // Retrieve the token from the cookie

            if (string.IsNullOrEmpty(token))
            {
                // Redirect to the login page or handle unauthorized access
                return RedirectToPage("/Account/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                Message = "Car deleted successfully.";
            }
            else
            {
                // Handle API error response
                Message = $"Failed to delete car. API Error: {response.StatusCode}";
            }

            return RedirectToPage();
        }
    }

    public class CarViewModel
    {
        public long Id { get; set; }
        public string CarNumber { get; set; }
        public string CarColor { get; set; }
        public string CarModel { get; set; }
        public DateTime RegistrationExpiryDate { get; set; }
        public int YearOfManufacture { get; set; }
        public string OwnerName { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
    }


    public class AttachmentViewModel
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
    }
    public class PaginatedCars
    {
        public IEnumerable<CarViewModel> Cars { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
