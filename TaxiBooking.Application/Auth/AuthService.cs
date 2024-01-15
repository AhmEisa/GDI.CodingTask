using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiBooking.Application.DTO;
using TaxiBooking.Infrastructure;

namespace TaxiBooking.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly TaxiBookingDbContext _dbContext;
        public AuthService(IConfiguration configuration, TaxiBookingDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<string> AuthenticateUser(LoginRequest model)
        {
            if (await IsValidUser(model))
            {
                return GenerateToken(model.Username);
            }
            return string.Empty;
        }

        private async Task<bool> IsValidUser(LoginRequest model)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Username == model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash)) { return false; }
            return true;
        }
        private string GenerateToken(string username)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }
    }
}
