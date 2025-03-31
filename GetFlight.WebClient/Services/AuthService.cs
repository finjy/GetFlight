using GetFlight.WebClient.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace GetFlight.WebClient.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public string Token { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Login(string username, string password)
        {
            var request = new LoginRequest { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>(_options);
                Token = result?.Token;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            Token = null;
        }
    }
}
