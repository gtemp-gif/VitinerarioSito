using Vitinerario.Models;
using Vitinerario.Models.Dtos;

namespace Vitinerario.Services
{
    public interface IApiService
    {
        //Task<List<EventViewModel>> GetEventsAsync();
        Task<List<EventDto>> GetEventsAsync(int langId);
        Task<List<ContentDto>> GetContentsByTypeAsync(string type, int langId);
        Task<List<ContentDto>> GetPodcastsAsync(int langId);
        Task<bool> SubmitProducerAsync(ProducerViewModel model);
        Task<bool> SubmitPartecipaAsync(PartecipaViewModel model);
        Task<ContentDto?> GetContentById(int id, int langId);
        Task<EventDto?> GetEventById(int id, int langId);
        Task<List<PartnerDto>> GetPartnersAsync();


        Task<TripDto?> GetTripByEventIdAsync(int eventId);
        Task<List<TripMustDto>> GetTripMustsAsync(int tripId);
        Task<List<StayDto>> GetStaysAsync(int tripId);
        Task<List<ItineraryDayDto>> GetItineraryDaysAsync(int tripId);
        Task<List<ItineraryStopDto>> GetItineraryStopsAsync(int dayId);

        Task<List<VariantPriceDto>> GetVariantPricesAsync(int eventId);
        Task<List<EventNeedDto>> GetEventNeedsAsync(int eventId);
    }
}
