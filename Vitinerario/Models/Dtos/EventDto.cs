namespace Vitinerario.Models.Dtos
{
    public sealed class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        // Date e Orari
        public DateTime EventDate { get; set; } // Mantenuto per compatibilità
        public DateTime StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndTime { get; set; }

        public string? CoverImage { get; set; }
        public DateTime? BookingEndDate { get; set; }
        public string? Location { get; set; }
        public string? Organizer { get; set; }
        public string? ContactInfo { get; set; }
        public decimal Price { get; set; }
        public bool IsOnline { get; set; }
        public int LangID { get; set; }
        public string? Subtitle { get; set; }
        public string? Coordinates { get; set; }
        public string? HeroImage { get; set; }
        public List<EventLinkDto> Links { get; set; } = new();
        public GalleryDto? Gallery { get; set; }
        public List<ExpertDto> Experts { get; set; } = new();
        public List<ExpertDto> Authors { get; set; } = new();
    }
}
