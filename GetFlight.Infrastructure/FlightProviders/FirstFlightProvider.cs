using GetFlight.Domain.Interfaces;
using GetFlight.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Infrastructure.FlightProviders
{
    public class FirstFlightProvider : IFlightProvider
    {
        private readonly ILogger<FirstFlightProvider> _logger;
        public string ProviderName => "FirstFlights";

        public FirstFlightProvider(ILogger<FirstFlightProvider> logger)
        {
            _logger = logger;
        }

        // Реализация методов IFlightProvider с моковыми данными
        public async Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime departureDate, int passengers, CancellationToken cancellationToken)
        {
            return null; // временно
        }

        public async Task<BookingResult> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            return null; // временно
        }
    }
}
