using System.Text.Json;
using Vitinerario.Models;
using Vitinerario.Models.Dtos;
using Vitinerario.Helpers;

namespace Vitinerario.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public ApiService(IHttpClientFactory httpClientFactory, IAuthService authService)
        {
            _httpClient = httpClientFactory.CreateClient("VitinerarioApi");
            _authService = authService;
        }

        private async Task AddAuthHeaderAsync()
        {
            var token = await _authService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            int langId = LanguageHelper.GetCurrentLangId();
            var dtos = await GetEventsAsync(langId);
            return dtos.Select(e => new EventViewModel
            {
                Id = e.Id.ToString(),
                Title = e.Title,
                Description = e.Description,
                Date = e.EventDate,
                Location = e.Location ?? string.Empty,
                ImageUrl = e.CoverImage ?? string.Empty
            }).ToList();
        }

        public async Task<List<EventDto>> GetEventsAsync(int langId)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"events?langId={langId}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<EventDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<List<EventDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return events ?? new List<EventDto>();
        }

        public async Task<List<ContentDto>> GetContentsByTypeAsync(string type, int langId)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"contents/type/{type}?langId={langId}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<ContentDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var contents = JsonSerializer.Deserialize<List<ContentDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return contents ?? new List<ContentDto>();
        }

        public async Task<List<ContentDto>> GetPodcastsAsync(int langId)
        {
            await AddAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"podcasts?langId={langId}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<ContentDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var podcasts = JsonSerializer.Deserialize<List<ContentDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return podcasts ?? new List<ContentDto>();
        }

        public async Task<bool> SubmitProducerAsync(ProducerViewModel model)
        {
            await AddAuthHeaderAsync();
            var content = new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Producers", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SubmitPartecipaAsync(PartecipaViewModel model)
        {
            await AddAuthHeaderAsync();
            var content = new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json");
            // Assuming the endpoint for participation is /EventParticipants or similar. Using 'partecipa' as standard fallback
            var response = await _httpClient.PostAsync("partecipa", content);
            return response.IsSuccessStatusCode;
        }
    }
}
