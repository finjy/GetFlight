using GetFlight.API.Controllers;
using GetFlight.Application.DTOs;
using GetFlight.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GetFlight.API.Tests.Controllers
{
    public class FlightsControllerTests
    {
        private readonly Mock<IFlightService> _mockFlightService;
        private readonly Mock<ILogger<FlightsController>> _mockLogger;
        private readonly FlightsController _controller;

        public FlightsControllerTests()
        {
            _mockFlightService = new Mock<IFlightService>();
            _mockLogger = new Mock<ILogger<FlightsController>>();
            _controller = new FlightsController(_mockFlightService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SearchFlights_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new SearchFlightRequest
            {
                Origin = "MOW",
                Destination = "LED",
                DepartureDate = DateTime.Now.AddDays(10),
                Passengers = 2
            };

            var expectedFlights = new List<FlightDto>
            {
                new FlightDto
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = "TEST123",
                    Origin = "MOW",
                    OriginName = "Moscow Airport",
                    Destination = "LED",
                    DestinationName = "St. Petersburg Airport",
                    DepartureTime = DateTime.Now.AddDays(10).AddHours(10),
                    ArrivalTime = DateTime.Now.AddDays(10).AddHours(12),
                    Airline = "Test Airlines",
                    Price = 100.5m,
                    AvailableSeats = 10,
                    Provider = "TestProvider"
                }
            };

            _mockFlightService.Setup(x => x.SearchFlightsAsync(
                    It.IsAny<SearchFlightRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFlights);

            // Act
            var result = await _controller.SearchFlights(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var flights = Assert.IsAssignableFrom<IEnumerable<FlightDto>>(okResult.Value);
            Assert.Single(flights);
            Assert.Equal("TEST123", flights.First().FlightNumber);
        }

        [Fact]
        public async Task BookFlight_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new BookingRequestDto
            {
                FlightId = Guid.NewGuid(),
                Provider = "TestProvider",
                Passengers = new[]
                {
                    new PassengerDto
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        PassportNumber = "AB123456"
                    }
                }
            };

            var expectedResult = new BookingResultDto
            {
                Success = true,
                BookingReference = "TEST1234",
                Message = "Booking successful"
            };

            _mockFlightService.Setup(x => x.BookFlightAsync(
                    It.IsAny<BookingRequestDto>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.BookFlight(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var bookingResult = Assert.IsAssignableFrom<BookingResultDto>(okResult.Value);
            Assert.True(bookingResult.Success);
            Assert.Equal("TEST1234", bookingResult.BookingReference);
        }
    }
}

