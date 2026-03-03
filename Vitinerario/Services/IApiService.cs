using Vitinerario.Models;
using Vitinerario.Models.Dtos;

namespace Vitinerario.Services
{
    public interface IApiService
    {
        Task<List<EventViewModel>> GetEventsAsync();
        Task<List<EventDto>> GetEventsAsync(int langId);
        Task<List<ContentDto>> GetContentsByTypeAsync(string type, int langId);
        Task<List<ContentDto>> GetPodcastsAsync(int langId);
        Task<bool> SubmitProducerAsync(ProducerViewModel model);
    }
}
