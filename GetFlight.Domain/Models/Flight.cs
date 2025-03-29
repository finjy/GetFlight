using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Domain.Models
{
    public class Flight
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; }
        public Airport Origin { get; set; }
        public Airport Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Airline { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public string Provider { get; set; } // Идентификатор источника данных
    }
}
