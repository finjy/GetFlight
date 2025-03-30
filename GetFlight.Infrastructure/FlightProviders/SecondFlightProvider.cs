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
    public class SecondFlightProvider : IFlightProvider
    {
        private readonly ILogger<SecondFlightProvider> _logger;
        private static readonly Random _random = new();

        public string ProviderName => "SecondFlights";

        public SecondFlightProvider(ILogger<SecondFlightProvider> logger)
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

            // Имитация задержки, иногда более длительной, чем у первого провайдера
            await Task.Delay(_random.Next(200, 1000), cancellationToken);

            // Генерация случайного количества рейсов
            var flights = new List<Flight>();
            int flightCount = _random.Next(2, 6);

            for (int i = 0; i < flightCount; i++)
            {
                // Базовое время вылета с добавлением случайных часов
                var departureTime = new DateTime(
                    departureDate.Year,
                    departureDate.Month,
                    departureDate.Day,
                    _random.Next(4, 23), // Часы с 4 утра до 23 вечера
                    _random.Next(0, 59), // Минуты
                    0); // Секунды

                // Время в пути от 1 до 6 часов (немного дольше, чем у FirstFlights)
                var flightDuration = TimeSpan.FromHours(_random.Next(1, 7) + _random.Next(0, 59) / 100.0);

                flights.Add(new Flight
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = $"SC{_random.Next(100, 999)}",
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
                    Airline = "Second Air",
                    Price = (decimal)(_random.Next(7500, 30000) / 100.0), // Цена от 75 до 300
                    AvailableSeats = _random.Next(3, 40),
                    Provider = ProviderName
                });
            }

            // Иногда добавляем "бонусный" рейс с очень низкой ценой
            if (_random.Next(100) < 30) // 30% шанс
            {
                var specialDepartureTime = new DateTime(
                    departureDate.Year,
                    departureDate.Month,
                    departureDate.Day,
                    _random.Next(0, 5), // Очень ранний рейс
                    _random.Next(0, 59),
                    0);

                flights.Add(new Flight
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = $"SC{_random.Next(100, 999)}",
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
                    DepartureTime = specialDepartureTime,
                    ArrivalTime = specialDepartureTime.AddHours(_random.Next(2, 8)),
                    Airline = "Second Air",
                    Price = (decimal)(_random.Next(5000, 7000) / 100.0), // Более низкая цена
                    AvailableSeats = _random.Next(1, 10), // Меньше мест
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
            await Task.Delay(_random.Next(300, 1200), cancellationToken);

            // 80% вероятность успешного бронирования (немного ниже, чем у FirstFlights)
            bool isSuccess = _random.Next(100) < 80;

            if (isSuccess)
            {
                string bookingRef = $"SC{DateTime.Now:yyMMdd}{_random.Next(10000, 99999)}";

                _logger.LogInformation("[{ProviderName}] Successfully booked flight {FlightId}. Reference: {BookingRef}",
                    ProviderName, request.FlightId, bookingRef);

                return new BookingResult
                {
                    Success = true,
                    BookingReference = bookingRef,
                    Message = "Your booking is confirmed"
                };
            }
            else
            {
                string errorMessage = _random.Next(2) == 0
                    ? "No available seats for the requested flight"
                    : "Unable to process payment at this time";

                _logger.LogWarning("[{ProviderName}] Failed to book flight {FlightId}. Reason: {Error}",
                    ProviderName, request.FlightId, errorMessage);

                return new BookingResult
                {
                    Success = false,
                    BookingReference = null,
                    Message = errorMessage
                };
            }
        }

        private string GetAirportName(string code)
        {
            return code switch
            {
                "MOW" => "Moscow Domodedovo Airport",
                "LED" => "Saint Petersburg Pulkovo Airport",
                "NYC" => "New York LaGuardia Airport",
                "LAX" => "Los Angeles Airport",
                "LON" => "London Gatwick Airport",
                "PAR" => "Paris Orly Airport",
                "BER" => "Berlin Tegel Airport",
                "ROM" => "Rome Ciampino Airport",
                "MAD" => "Madrid Airport",
                _ => $"{code} Airport"
            };
        }
    }
}
