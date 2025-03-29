using Microsoft.Extensions.DependencyInjection;
using GetFlight.Application.Interfaces;

namespace GetFlight.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Регистрация сервисов приложения
            services.AddTransient<IFlightService, FlightService>();

            return services;
        }
    }
}
