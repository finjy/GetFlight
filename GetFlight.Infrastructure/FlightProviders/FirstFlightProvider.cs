using GetFlight.Domain.Interfaces;
using GetFlight.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFlight.Infrastructure.FlightProviders
{
    public class FirstFlightProvider : IFlightProvider
    {
        private readonly ILogger<FirstFlightProvider> _logger;
        private static readonly Random _random = new();

        public string ProviderName => "FirstFlights";

        public FirstFlightProvider(ILogger<FirstFlightProvider> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Flight>> SearchFlightsAsync(
            string origin,
            string destination,
            DateTime departureDate,
            int passengers,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("[{ProviderName}] Searching flights from {Origin} to {Destination} on {Date}",
                ProviderName, origin, destination, departureDate.ToString("yyyy-MM-dd"));

            // Имитация задержки сетевого запроса
            await Task.Delay(_random.Next(100, 500), cancellationToken);

            // Генерация случайного количества рейсов
            var flights = new List<Flight>();
            int flightCount = _random.Next(3, 8);

            for (int i = 0; i < flightCount; i++)
            {
                // Базовое время вылета с добавлением случайных часов
                var departureTime = new DateTime(
                    departureDate.Year,
                    departureDate.Month,
                    departureDate.Day,
                    _random.Next(6, 22), // Часы с 6 утра до 22 вечера
                    _random.Next(0, 59), // Минуты
                    0); // Секунды

                // Время в пути от 1 до 5 часов
                var flightDuration = TimeSpan.FromHours(_random.Next(1, 6) + _random.Next(0, 59) / 100.0);

                flights.Add(new Flight
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = $"FF{_random.Next(1000, 9999)}",
                    Origin = new Airport
                    {
                        Code = origin,
                        Name = GetAirportName(origin),
                        City = origin,
                        Country = "Country"
                    },
                    Destination = new Airport
                    {
                        Code = destination,
                        Name = GetAirportName(destination),
                        City = destination,
                        Country = "Country"
                    },
                    DepartureTime = departureTime,
                    ArrivalTime = departureTime.Add(flightDuration),
                    Airline = "First Airlines",
                    Price = (decimal)(_random.Next(8000, 25000) / 100.0), // Цена от 80 до 250
                    AvailableSeats = _random.Next(5, 50),
                    Provider = ProviderName
                });
            }

            _logger.LogInformation("[{ProviderName}] Found {FlightCount} flights",
                ProviderName, flights.Count);

            return flights;
        }

        public async Task<BookingResult> BookFlightAsync(
            BookingRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("[{ProviderName}] Booking flight {FlightId} for {NumberOfSeats} passengers",
                ProviderName, request.FlightId, request.NumberOfSeats);

            // Имитация задержки сетевого запроса
            await Task.Delay(_random.Next(200, 1000), cancellationToken);

            // 90% вероятность успешного бронирования
            bool isSuccess = _random.Next(100) < 90;

            if (isSuccess)
            {
                string bookingRef = $"{ProviderName.Substring(0, 2)}{DateTime.Now:yyMMdd}{_random.Next(1000, 9999)}";

                _logger.LogInformation("[{ProviderName}] Successfully booked flight {FlightId}. Reference: {BookingRef}",
                    ProviderName, request.FlightId, bookingRef);

                return new BookingResult
                {
                    Success = true,
                    BookingReference = bookingRef,
                    Message = "Booking successful"
                };
            }
            else
            {
                _logger.LogWarning("[{ProviderName}] Failed to book flight {FlightId}. No available seats.",
                    ProviderName, request.FlightId);

                return new BookingResult
                {
                    Success = false,
                    BookingReference = null,
                    Message = "No available seats for the requested flight"
                };
            }
        }

        private string GetAirportName(string code)
        {
            return code switch
            {
                "MOW" => "Moscow Sheremetyevo Airport",
                "LED" => "Saint Petersburg Airport",
                "NYC" => "New York JFK Airport",
                "LAX" => "Los Angeles International Airport",
                "LON" => "London Heathrow Airport",
                "PAR" => "Paris Charles de Gaulle Airport",
                "BER" => "Berlin Brandenburg Airport",
                "ROM" => "Rome Fiumicino Airport",
                "MAD" => "Madrid Barajas Airport",
                _ => $"{code} International Airport"
            };
        }
    }
}
