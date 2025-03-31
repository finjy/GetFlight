namespace GetFlight.WebClient.Models
{
    public class PassengerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-30);
        public string PassportNumber { get; set; }
    }
}
