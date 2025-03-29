using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Application.DTOs
{
    public class BookingRequestDto
    {
        public Guid FlightId { get; set; }
        public string Provider { get; set; }
        public PassengerDto[] Passengers { get; set; }
    }
}
