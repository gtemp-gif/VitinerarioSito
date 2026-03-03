namespace Vitinerario.Models.Dtos
{
    public sealed class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public string? CoverImage { get; set; }
        public DateTime? BookingEndDate { get; set; }
        public string? Location { get; set; }
        public string? Organizer { get; set; }
        public string? ContactInfo { get; set; }
        public decimal Price { get; set; }
        public bool IsOnline { get; set; }
        public int LangID { get; set; }
        public List<EventLinkDto> Links { get; set; } = new();
        public GalleryDto? Gallery { get; set; }
        public List<ExpertDto> Experts { get; set; } = new();
    }
}
