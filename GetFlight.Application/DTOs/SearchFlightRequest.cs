using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Application.DTOs
{
    public class SearchFlightRequest
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        public int Passengers { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Airline { get; set; }
        public string SortBy { get; set; } = "Price";
        public string SortOrder { get; set; } = "Asc";
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}
