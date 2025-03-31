namespace GetFlight.WebClient.Models
{
    public class SearchFlightRequest
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureDate { get; set; } = DateTime.Now.AddDays(1);
        public int Passengers { get; set; } = 1;
        public decimal? MaxPrice { get; set; }
        public string Airline { get; set; }
    }
}
