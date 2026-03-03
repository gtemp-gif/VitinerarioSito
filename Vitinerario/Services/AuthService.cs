using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using Vitinerario.Models.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace Vitinerario.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly IMemoryCache _memoryCache;
        private const string TokenCacheKey = "VitinerarioAuthToken";

        public AuthService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _memoryCache = memoryCache;
        }

        public async Task<string> GetTokenAsync()
        {
           // var url = $"{_apiSettings.BaseUrl.TrimEnd('/')}/Auth/token";
           var url = $"{_apiSettings.BaseUrl.TrimEnd('/')}/Auth/login";
            // Definisci l'oggetto anonimo con la proprietà 'token' in minuscolo
            //var requestBody = new { token = _apiSettings.Secret };
            var requestBody = new { username = "VIT.Site2026", password = "Test1234" };
                // Invia la richiesta POST
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to retrieve token. Status: {response.StatusCode}. Details: {errorMsg}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Token))
            {
                throw new Exception("Token not found in the authentication response.");
            }

            // Assuming token is valid for a certain duration. Defaulting to 1 hour if not provided by API.
            // Some APIs return "expires_in" in seconds. If the API provides it, update this logic.
            // Set expiration slightly before actual expiration (e.g., 5 mins before)
            var expirationTime = DateTimeOffset.UtcNow.AddHours(1).AddMinutes(-5);

            _memoryCache.Set(TokenCacheKey, tokenResponse.Token, expirationTime);

            return tokenResponse.Token;
        }

        private class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
            public int ExpiresIn { get; set; } // Example property if the API returns expiration time
        }
    }
}
