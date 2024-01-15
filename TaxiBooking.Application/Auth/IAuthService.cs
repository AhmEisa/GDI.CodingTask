using TaxiBooking.Application.DTO;

namespace TaxiBooking.Application.Auth
{
    public interface IAuthService
    {
        Task<string> AuthenticateUser(LoginRequest model);
    }
}
