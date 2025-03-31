namespace GetFlight.WebClient.Models
{
    public class BookingRequestDto
    {
        public Guid FlightId { get; set; }
        public string Provider { get; set; }
        public PassengerDto[] Passengers { get; set; }
    }
}
