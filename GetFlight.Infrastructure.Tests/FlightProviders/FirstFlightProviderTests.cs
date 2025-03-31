using GetFlight.Domain.Models;
using GetFlight.Infrastructure.FlightProviders;
using Microsoft.Extensions.Logging;
using Moq;

namespace GetFlight.Infrastructure.Tests.FlightProviders
{
    public class FirstFlightProviderTests
    {
        private readonly Mock<ILogger<FirstFlightProvider>> _mockLogger;
        private readonly FirstFlightProvider _provider;

        public FirstFlightProviderTests()
        {
            _mockLogger = new Mock<ILogger<FirstFlightProvider>>();
            _provider = new FirstFlightProvider(_mockLogger.Object);
        }

        [Fact]
        public void ProviderName_ShouldBeFirstFlights()
        {
            // Assert
            Assert.Equal("FirstFlights", _provider.ProviderName);
        }

        [Fact]
        public async Task SearchFlightsAsync_ShouldReturnFlights()
        {
            // Arrange
            var origin = "MOW";
            var destination = "LED";
            var departureDate = DateTime.Now.AddDays(7);
            var passengers = 2;

            // Act
            var result = await _provider.SearchFlightsAsync(
                origin,
                destination,
                departureDate,
                passengers,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            foreach (var flight in result)
            {
                Assert.Equal(origin, flight.Origin.Code);
                Assert.Equal(destination, flight.Destination.Code);
                Assert.Equal(departureDate.Date, flight.DepartureTime.Date);
                Assert.Equal("FirstFlights", flight.Provider);
                Assert.True(flight.AvailableSeats >= passengers);
            }
        }

        [Fact]
        public async Task BookFlightAsync_ShouldReturnBookingResult()
        {
            // Arrange
            var request = new BookingRequest
            {
                FlightId = Guid.NewGuid(),
                Provider = "FirstFlights",
                NumberOfSeats = 2,
                Passengers = new[]
                {
                    new Passenger
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        PassportNumber = "AB123456"
                    },
                    new Passenger
                    {
                        FirstName = "Jane",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(1992, 5, 10),
                        PassportNumber = "CD789012"
                    }
                }
            };

            // Act
            var result = await _provider.BookFlightAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            // Поскольку провайдер имеет случайное поведение, мы можем только проверить, что поля заполнены
            Assert.NotNull(result.BookingReference);
            Assert.NotEmpty(result.BookingReference);
            Assert.NotNull(result.Message);
            Assert.NotEmpty(result.Message);
        }
    }
}
