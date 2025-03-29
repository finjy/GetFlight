using GetFlight.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Application.Interfaces
{
    public interface IFlightService
    {
        Task<IEnumerable<FlightDto>> SearchFlightsAsync(SearchFlightRequest request, CancellationToken cancellationToken);

        Task<BookingResultDto> BookFlightAsync(BookingRequestDto request, CancellationToken cancellationToken);
    }
}
