using GetFlight.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Domain.Interfaces
{
    public interface IFlightProvider
    {
        string ProviderName { get; }

        Task<IEnumerable<Flight>> SearchFlightsAsync(
            string origin,
            string destination,
            DateTime departureDate,
            int passengers,
            CancellationToken cancellationToken);

        Task<BookingResult> BookFlightAsync(
            BookingRequest request,
            CancellationToken cancellationToken);
    }
}
