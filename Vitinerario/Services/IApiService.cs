using Vitinerario.Models;

namespace Vitinerario.Services
{
    public interface IApiService
    {
        Task<List<EventViewModel>> GetEventsAsync();
        Task<bool> SubmitProducerAsync(ProducerViewModel model);
    }
}
