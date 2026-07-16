namespace Vitinerario.Models.Dtos
{
    
    public class TripDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string? DepartureCity { get; set; }
        public string? DepartureCountry { get; set; }
        public string? ArrivalCity { get; set; }
        public string? ArrivalCountry { get; set; }
        public int DurationDays { get; set; }
        public int DurationNights { get; set; }
        public int MaxGuests { get; set; }
        public string? Status { get; set; }
    }

    

    public class TripMustDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string Text { get; set; } = null!;
        public int TypeId { get; set; } // 1 = Include, 2 = Exclude
    }

    public class StayDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Location { get; set; }
        public int OrderIndex { get; set; }
        public int? ItineraryDayId { get; set; }
    }

    public class ItineraryDayDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int DayNumber { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        // Aggiungi queste 3 proprietà per mappare il DB
        public string Image1 { get; set; } = string.Empty;
        public string Image2 { get; set; } = string.Empty;
        public string Image3 { get; set; } = string.Empty;
    }

    public class ItineraryStopDto
    {
        public int Id { get; set; }
        public int DayId { get; set; }
        public TimeSpan Time { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int OrderIndex { get; set; }
    }
}