namespace Vitinerario.Models.Dtos
{
    public sealed class ContentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        public string? CoverImage { get; set; }
        public string ContentType { get; set; } = null!;
        public bool IsPublished { get; set; }
        public string? Preview { get; set; }
        public string? HeroImage { get; set; }
        public int? CategoryId { get; set; }
       
        public List<ContentImageDto> Images { get; set; } = new();
        public List<ContentLinkDto> Links { get; set; } = new();
        public List<ExpertDto> Authors { get; set; } = new();
    }
}
