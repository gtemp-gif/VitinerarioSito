using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Vitinerario.Helpers;
using Vitinerario.Models;
using Vitinerario.Models.Dtos;
using Vitinerario.Models.Settings;

namespace Vitinerario.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ApiSettings _apiSettings;
        // Rimuovi IMemoryCache se non la usi per altro, AuthService gestisce già il token

        public ApiService(IHttpClientFactory httpClientFactory,
                          IAuthService authService,
                          IOptions<ApiSettings> apiSettings) // Usa IOptions qui
        {
            _httpClient = httpClientFactory.CreateClient("VitinerarioApi");
            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "VitinerarioWebApp");
            }
            _authService = authService;
            _apiSettings = apiSettings.Value; // Estrai il valore qui
        }

        private async Task AddAuthHeaderAsync()
        {
            // Non serve controllare la cache qui! 
            // Il metodo GetTokenAsync() del tuo AuthService è già ottimizzato 
            // per restituire il token dalla cache se presente.
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
            var url = $"{_apiSettings.BaseUrl.TrimEnd('/')}";
            var response = await _httpClient.GetAsync($"{url}/events?langId={langId}");

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
            var url = $"{_apiSettings.BaseUrl.TrimEnd('/')}";
            // var response = await _httpClient.GetAsync($"contents/type/{type}?langId={langId}");
            var response = await _httpClient.GetAsync($"{url}/contents/type/{type}");
            if (!response.IsSuccessStatusCode)
            {
                return new List<ContentDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var contents = JsonSerializer.Deserialize<List<ContentDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return contents ?? new List<ContentDto>();
        }

        public async Task<ContentDto> GetContentById(int id, int langId)
        {
            await AddAuthHeaderAsync();
            var url = $"{_apiSettings.BaseUrl.TrimEnd('/')}";
            // var response = await _httpClient.GetAsync($"contents/type/{type}?langId={langId}");
            var response = await _httpClient.GetAsync($"{url}/contents/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new ContentDto();
            }

            var content = await response.Content.ReadAsStringAsync();
            var contents = JsonSerializer.Deserialize<ContentDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return contents ?? new ContentDto();
        }

        public async Task<List<ContentDto>> GetPodcastsAsync(int langId)
        {
            await AddAuthHeaderAsync();
            var url = $"{_apiSettings.BaseUrl.TrimEnd('/')}";
            var response = await _httpClient.GetAsync($"{url}/podcasts");

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
