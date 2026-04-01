using System.Collections.Generic;
using Vitinerario.Models.Dtos;

namespace Vitinerario.Models
{
    public class TravelEventViewModel
    {
        public EventDto Event { get; set; } = null!;
        public TripDto? Trip { get; set; }

        public List<TripMustDto> Musts { get; set; } = new();
        public List<StayDto> Stays { get; set; } = new();

        // Raggruppa i giorni con le loro rispettive tappe orarie
        public List<FullItineraryDay> Itinerary { get; set; } = new();
    }

    public class FullItineraryDay
    {
        public ItineraryDayDto Day { get; set; } = null!;
        public List<ItineraryStopDto> Stops { get; set; } = new();
    }
}