using GetFlight.Application.DTOs;
using GetFlight.Domain.Interfaces;
using GetFlight.Domain.Models;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Application.Tests.Services
{
    public class FlightServiceTests
    {
        private readonly Mock<IFlightProvider> _mockProvider1;
        private readonly Mock<IFlightProvider> _mockProvider2;
        private readonly IAppCache _realCache;  // Используем реальный кэш
        private readonly Mock<ILogger<FlightService>> _mockLogger;
        private readonly FlightService _service;

        public FlightServiceTests()
        {
            _mockProvider1 = new Mock<IFlightProvider>();
            _mockProvider1.Setup(p => p.ProviderName).Returns("Provider1");

            _mockProvider2 = new Mock<IFlightProvider>();
            _mockProvider2.Setup(p => p.ProviderName).Returns("Provider2");

            _realCache = new CachingService();  // Используем реальную реализацию
            _mockLogger = new Mock<ILogger<FlightService>>();

            var providers = new List<IFlightProvider> { _mockProvider1.Object, _mockProvider2.Object };
            _service = new FlightService(providers, _realCache, _mockLogger.Object);
        }

        [Fact]
        public async Task SearchFlightsAsync_WithMultipleProviders_AggregatesToSingleList()
        {
            // Arrange
            var request = new SearchFlightRequest
            {
                Origin = "MOW",
                Destination = "LED",
                DepartureDate = DateTime.Now.AddDays(10),
                Passengers = 2
            };

            var provider1Flights = new List<Flight>
            {
                new Flight
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = "P1-123",
                    Origin = new Airport { Code = "MOW", Name = "Moscow" },
                    Destination = new Airport { Code = "LED", Name = "St. Petersburg" },
                    DepartureTime = DateTime.Now.AddDays(10),
                    ArrivalTime = DateTime.Now.AddDays(10).AddHours(2),
                    Airline = "Provider1 Airlines",
                    Price = 100m,
                    AvailableSeats = 10,
                    Provider = "Provider1"
                }
            };

            var provider2Flights = new List<Flight>
            {
                new Flight
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = "P2-456",
                    Origin = new Airport { Code = "MOW", Name = "Moscow" },
                    Destination = new Airport { Code = "LED", Name = "St. Petersburg" },
                    DepartureTime = DateTime.Now.AddDays(10),
                    ArrivalTime = DateTime.Now.AddDays(10).AddHours(2),
                    Airline = "Provider2 Airlines",
                    Price = 150m,
                    AvailableSeats = 5,
                    Provider = "Provider2"
                }
            };

            _mockProvider1.Setup(p => p.SearchFlightsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(provider1Flights);

            _mockProvider2.Setup(p => p.SearchFlightsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(provider2Flights);

            // Act
            var result = await _service.SearchFlightsAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, f => f.FlightNumber == "P1-123");
            Assert.Contains(result, f => f.FlightNumber == "P2-456");
        }

        [Fact]
        public async Task SearchFlightsAsync_WithFilters_FiltersFlightsByPrice()
        {
            // Arrange
            var request = new SearchFlightRequest
            {
                Origin = "MOW",
                Destination = "LED",
                DepartureDate = DateTime.Now.AddDays(10),
                Passengers = 2,
                MaxPrice = 120m // Должен отфильтровать второй рейс
            };

            var provider1Flights = new List<Flight>
            {
                new Flight
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = "P1-123",
                    Origin = new Airport { Code = "MOW", Name = "Moscow" },
                    Destination = new Airport { Code = "LED", Name = "St. Petersburg" },
                    DepartureTime = DateTime.Now.AddDays(10),
                    ArrivalTime = DateTime.Now.AddDays(10).AddHours(2),
                    Airline = "Provider1 Airlines",
                    Price = 100m,
                    AvailableSeats = 10,
                    Provider = "Provider1"
                }
            };

            var provider2Flights = new List<Flight>
            {
                new Flight
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = "P2-456",
                    Origin = new Airport { Code = "MOW", Name = "Moscow" },
                    Destination = new Airport { Code = "LED", Name = "St. Petersburg" },
                    DepartureTime = DateTime.Now.AddDays(10),
                    ArrivalTime = DateTime.Now.AddDays(10).AddHours(2),
                    Airline = "Provider2 Airlines",
                    Price = 150m, // Цена выше фильтра
                    AvailableSeats = 5,
                    Provider = "Provider2"
                }
            };

            _mockProvider1.Setup(p => p.SearchFlightsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(provider1Flights);

            _mockProvider2.Setup(p => p.SearchFlightsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(provider2Flights);

            // Act
            var result = await _service.SearchFlightsAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Только один рейс должен пройти фильтрацию
            Assert.Equal("P1-123", result.First().FlightNumber);
            Assert.Equal(100m, result.First().Price);
        }

        [Fact]
        public async Task BookFlightAsync_CallsCorrectProvider()
        {
            // Arrange
            var flightId = Guid.NewGuid();
            var bookingRequest = new BookingRequestDto
            {
                FlightId = flightId,
                Provider = "Provider1", // Должен вызвать первый провайдер
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

            var expectedResult = new BookingResult
            {
                Success = true,
                BookingReference = "TEST1234",
                Message = "Booking successful"
            };

            _mockProvider1.Setup(p => p.BookFlightAsync(
                    It.Is<BookingRequest>(br =>
                        br.FlightId == flightId &&
                        br.Provider == "Provider1" &&
                        br.NumberOfSeats == 1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.BookFlightAsync(bookingRequest, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("TEST1234", result.BookingReference);

            // Проверяем, что вызывался только первый провайдер
            _mockProvider1.Verify(p => p.BookFlightAsync(
                It.IsAny<BookingRequest>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _mockProvider2.Verify(p => p.BookFlightAsync(
                It.IsAny<BookingRequest>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
