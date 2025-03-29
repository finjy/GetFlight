using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Application.DTOs
{
    public class FlightDto
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string OriginName { get; set; }
        public string Destination { get; set; }
        public string DestinationName { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Airline { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public string Provider { get; set; }
        public TimeSpan Duration => ArrivalTime - DepartureTime;
    }
}
