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
    public class SecondFlightProvider : IFlightProvider
    {
        private readonly ILogger<SecondFlightProvider> _logger;

        public string ProviderName => "SecondFlights";

        public SecondFlightProvider(ILogger<SecondFlightProvider> logger)
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
