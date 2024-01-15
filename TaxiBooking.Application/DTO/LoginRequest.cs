#nullable disable
using System.ComponentModel.DataAnnotations;

namespace TaxiBooking.Application.DTO
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
