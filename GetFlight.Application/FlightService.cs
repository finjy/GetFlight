using GetFlight.Application.DTOs;
using GetFlight.Application.Interfaces;
using GetFlight.Domain.Interfaces;
using GetFlight.Domain.Models;
using LazyCache;
using Microsoft.Extensions.Logging;

namespace GetFlight.Application
{
    public class FlightService : IFlightService
    {
        private readonly IEnumerable<IFlightProvider> _flightProviders;
        private readonly IAppCache _cache;
        private readonly ILogger<FlightService> _logger;

        public FlightService(IEnumerable<IFlightProvider> flightProviders, IAppCache cache, ILogger<FlightService> logger)
        {
            _flightProviders = flightProviders ?? throw new ArgumentNullException(nameof(flightProviders));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<FlightDto>> SearchFlightsAsync(SearchFlightRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // формируем ключ кэша
            var cacheKey = $"flights_{request.Origin}_{request.Destination}_{request.DepartureDate:dd-MM-yyyy}_{request.Passengers}";

            // с LazyCache используем GetOrAddAsync 
            var flights = await _cache.GetOrAddAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);

                _logger.LogInformation("Cache miss. Searching flights from providers for route {Origin} to {Destination} on {DepartureDate}", request.Origin, request.Destination, request.DepartureDate.ToString("dd-MM-yyyy"));

                // создаем задачи для параллельного запроса к провайдерам
                var searchTasks = _flightProviders.Select(provider =>
                    SearchWithTimeoutAsync(provider, request, cancellationToken)).ToList();

                // ждем завершения всех задач
                var results = await Task.WhenAll(searchTasks);

                // объединяем результаты и преобразуем в DTO
                return results
                    .SelectMany(flights => flights)
                    .Select(flight => MapToDto(flight))
                    .ToList();
            });

            _logger.LogInformation("Found {FlightCount} flights for route {Origin} to {Destination}",
                flights.Count(), request.Origin, request.Destination);

            return ApplyFiltersAndSort(flights, request);
        }

        private async Task<IEnumerable<Flight>> SearchWithTimeoutAsync(IFlightProvider provider, SearchFlightRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Requesting flights from provider {ProviderName}", provider.ProviderName);

                // устанавливаем таймаут для запроса
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(5)); // таймаут 5 секунд

                return await provider.SearchFlightsAsync(
                    request.Origin,
                    request.Destination,
                    request.DepartureDate,
                    request.Passengers,
                    timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Timeout occurred while getting flights from provider {ProviderName}", provider.ProviderName);
                return Enumerable.Empty<Flight>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting flights from provider {ProviderName}", provider.ProviderName);
                return Enumerable.Empty<Flight>();
            }
        }

        public async Task<BookingResultDto> BookFlightAsync(BookingRequestDto request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            _logger.LogInformation("Booking flight {FlightId} from provider {Provider}", request.FlightId, request.Provider);

            // находим нужного провайдера
            var provider = _flightProviders.FirstOrDefault(p => p.ProviderName.Equals(request.Provider, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
            {
                _logger.LogWarning("Provider {Provider} not found", request.Provider);
                return new BookingResultDto
                {
                    Success = false,
                    Message = $"Provider {request.Provider} not found"
                };
            }

            try
            {
                // создаем запрос на бронирование
                var bookingRequest = new BookingRequest
                {
                    FlightId = request.FlightId,
                    Provider = request.Provider,
                    NumberOfSeats = request.Passengers.Length,
                    Passengers = request.Passengers.Select(p => new Passenger
                    {
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        DateOfBirth = p.DateOfBirth,
                        PassportNumber = p.PassportNumber
                    }).ToArray()
                };

                // выполняем бронирование
                var result = await provider.BookFlightAsync(bookingRequest, cancellationToken);

                if (result.Success)
                {
                    _logger.LogInformation("Successfully booked flight {FlightId}. Booking reference: {BookingReference}",
                        request.FlightId, result.BookingReference);
                }
                else
                {
                    _logger.LogWarning("Failed to book flight {FlightId}. Reason: {Message}",
                        request.FlightId, result.Message);
                }

                return new BookingResultDto
                {
                    Success = result.Success,
                    BookingReference = result.BookingReference,
                    Message = result.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while booking flight {FlightId}", request.FlightId);
                return new BookingResultDto
                {
                    Success = false,
                    Message = $"Booking failed: {ex.Message}"
                };
            }
        }

        private IEnumerable<FlightDto> ApplyFiltersAndSort(IEnumerable<FlightDto> flights, SearchFlightRequest request)
        {
            var filteredFlights = flights;

            // применяем фильтры
            if (request.MaxPrice.HasValue)
            {
                filteredFlights = filteredFlights.Where(f => f.Price <= request.MaxPrice.Value);
                _logger.LogDebug("Applied price filter: maximum {MaxPrice}", request.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(request.Airline))
            {
                filteredFlights = filteredFlights.Where(f =>
                    f.Airline.Equals(request.Airline, StringComparison.OrdinalIgnoreCase));
                _logger.LogDebug("Applied airline filter: {Airline}", request.Airline);
            }

            // применяем сортировку
            IEnumerable<FlightDto> sortedFlights = request.SortBy?.ToLower() switch
            {
                "price" => request.SortOrder?.ToLower() == "desc"
                    ? filteredFlights.OrderByDescending(f => f.Price)
                    : filteredFlights.OrderBy(f => f.Price),

                "duration" => request.SortOrder?.ToLower() == "desc"
                    ? filteredFlights.OrderByDescending(f => f.Duration)
                    : filteredFlights.OrderBy(f => f.Duration),

                "departure" => request.SortOrder?.ToLower() == "desc"
                    ? filteredFlights.OrderByDescending(f => f.DepartureTime)
                    : filteredFlights.OrderBy(f => f.DepartureTime),

                "arrival" => request.SortOrder?.ToLower() == "desc"
                    ? filteredFlights.OrderByDescending(f => f.ArrivalTime)
                    : filteredFlights.OrderBy(f => f.ArrivalTime),

                _ => filteredFlights.OrderBy(f => f.Price)
            };

            _logger.LogDebug("Applied sorting: {SortBy} {SortOrder}",
                request.SortBy ?? "price", request.SortOrder ?? "asc");

            // применяем пагинацию
            if (request.Offset.HasValue)
            {
                sortedFlights = sortedFlights.Skip(request.Offset.Value);
                _logger.LogDebug("Applied offset: {Offset}", request.Offset.Value);
            }

            if (request.Limit.HasValue)
            {
                sortedFlights = sortedFlights.Take(request.Limit.Value);
                _logger.LogDebug("Applied limit: {Limit}", request.Limit.Value);
            }

            return sortedFlights;
        }

        private FlightDto MapToDto(Flight flight)
        {
            return new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin.Code,
                OriginName = flight.Origin.Name,
                Destination = flight.Destination.Code,
                DestinationName = flight.Destination.Name,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Airline = flight.Airline,
                Price = flight.Price,
                AvailableSeats = flight.AvailableSeats,
                Provider = flight.Provider
            };
        }
    }
}

