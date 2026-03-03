namespace Vitinerario.Models.Dtos
{
    public sealed class EventLinkDto
    {
        public int Id { get; set; }
        public string LinkUrl { get; set; } = null!;
        public string? Description { get; set; }
    }
}
