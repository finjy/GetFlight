using Microsoft.Extensions.DependencyInjection;
using GetFlight.Domain.Interfaces;
using GetFlight.Infrastructure.FlightProviders;

namespace GetFlight.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Регистрация провайдеров полетов
            services.AddTransient<IFlightProvider, FirstFlightProvider>();
            services.AddTransient<IFlightProvider, SecondFlightProvider>();

            return services;
        }
    }
}
