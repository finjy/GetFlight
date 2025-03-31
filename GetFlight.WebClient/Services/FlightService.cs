using GetFlight.WebClient.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GetFlight.WebClient.Services
{
    public class FlightService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public FlightService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<IEnumerable<FlightDto>> SearchFlights(SearchFlightRequest request)
        {
            var query = $"api/flights?origin={request.Origin}&destination={request.Destination}" +
                        $"&departureDate={request.DepartureDate:yyyy-MM-dd}&passengers={request.Passengers}";

            if (request.MaxPrice.HasValue)
                query += $"&maxPrice={request.MaxPrice.Value}";

            if (!string.IsNullOrEmpty(request.Airline))
                query += $"&airline={Uri.EscapeDataString(request.Airline)}";

            var response = await _httpClient.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<FlightDto>>(_options) ?? new List<FlightDto>();
            }

            return new List<FlightDto>();
        }

        public async Task<BookingResultDto> BookFlight(BookingRequestDto request)
        {
            if (!_authService.IsAuthenticated)
                return new BookingResultDto { Success = false, Message = "User not authenticated" };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);

            var response = await _httpClient.PostAsJsonAsync("api/flights/book", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BookingResultDto>(_options) ??
                       new BookingResultDto { Success = false, Message = "Failed to parse response" };
            }

            return new BookingResultDto
            {
                Success = false,
                Message = $"Error: {response.StatusCode}"
            };
        }
    }
}
