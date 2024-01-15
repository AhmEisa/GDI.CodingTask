using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using TaxiBooking.Application.Auth;
using TaxiBooking.Application.DTO;

namespace TaxiBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var existingToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
            if (!string.IsNullOrEmpty(existingToken))
            {
                return Ok(new { Token = existingToken });
            }
            var token = await _authService.AuthenticateUser(model);
            return string.IsNullOrEmpty(token) ?
                Unauthorized(new { Message = "Invalid username or password" }) :
                Ok(new { Token = token });
        }
    }
}