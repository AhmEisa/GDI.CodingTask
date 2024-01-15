using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace TaxiBooking.Web.Pages.Cars
{
    [Authorize]
    public class AddModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public AddModel(IConfiguration configuration, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public CarViewModel Car { get; set; }

        [TempData]
        public string Message { get; set; }

        [TempData]
        public string AlertType { get; set; }

        public void OnGet()
        {
            // Initialize model properties if needed
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile> Attachments)
        {
            if (ModelState.IsValid)
            {
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/api/car"; // Replace with your actual API endpoint
                var token = HttpContext.Request.Cookies["AuthToken"]; // Retrieve the token from the cookie

                if (string.IsNullOrEmpty(token))
                {
                    // Redirect to the login page or handle unauthorized access
                    return RedirectToPage("/Account/Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Handle file uploads
                if (Attachments != null && Attachments.Count > 0)
                {
                    Car.Attachments = new List<AttachmentViewModel>();

                    foreach (var file in Attachments)
                    {
                        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(file.FileName);

                        if (!Array.Exists(allowedExtensions, ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
                        {
                            ModelState.AddModelError("Car.Attachments", "Invalid file format. Allowed formats: PDF, JPG, JPEG, PNG.");
                            return Page();
                        }

                        if (file.Length > 10 * 1024 * 1024) // 10 MB limit
                        {
                            ModelState.AddModelError("Car.Attachments", "File size exceeds the maximum limit (10 MB).");
                            return Page();
                        }

                        // Save the file to the server
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Add file details to the Car model
                        Car.Attachments.Add(new AttachmentViewModel { FileName = file.FileName, Url = $"~/uploads/{uniqueFileName}" });
                    }
                }

                var carJson = JsonSerializer.Serialize(Car);
                var content = new StringContent(carJson, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Message = "Car added successfully.";
                    AlertType = "alert-success";
                    return RedirectToPage("./Index");
                }
                else
                {
                    // Handle API error response
                    Message = $"Failed to add car. API Error: {response.StatusCode}";
                    AlertType = "alert-danger";
                    return Page();
                }
            }

            return Page();
        }
    }
}
