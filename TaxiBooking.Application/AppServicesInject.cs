using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using TaxiBooking.Application.Auth;
using TaxiBooking.Application.Cars;
using TaxiBooking.Application.DTO;
using TaxiBooking.Application.Mapping;
using TaxiBooking.Application.Validations;

namespace TaxiBooking.Application
{
    public static class AppServicesInject
    {
        /// <summary>
        /// Configure Application Services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureAppServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddScoped<IValidator<CarViewModel>, CarViewModelValidator>();
            return services;
        }
    }
}
