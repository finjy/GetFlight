using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Domain.Models
{
    public class BookingRequest
    {
        public Guid FlightId { get; set; }
        public string Provider { get; set; }
        public int NumberOfSeats { get; set; }
        public Passenger[] Passengers { get; set; }
    }
}
