using System.Text.Json;
using Vitinerario.Models;

namespace Vitinerario.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("VitinerarioApi");
        }

        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            // Assuming the endpoint for events is /Events
            var response = await _httpClient.GetAsync("Events");

            if (!response.IsSuccessStatusCode)
            {
                // Better error handling should be in place, returning an empty list for now.
                return new List<EventViewModel>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<List<EventViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return events ?? new List<EventViewModel>();
        }

        public async Task<bool> SubmitProducerAsync(ProducerViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Producers", content);
            return response.IsSuccessStatusCode;
        }
    }
}
