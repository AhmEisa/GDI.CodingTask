using Microsoft.Extensions.DependencyInjection;

namespace TaxiBooking.Infrastructure
{
    public static class AppInfraInject
    {
        /// <summary>
        /// Configure Application Infrastructure
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureAppInfra(this IServiceCollection services)
        {
            services.AddDbContext<TaxiBookingDbContext>();

            return services;
        }
    }
}
