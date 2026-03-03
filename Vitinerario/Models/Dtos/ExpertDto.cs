namespace Vitinerario.Models.Dtos
{
    public sealed class ExpertDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Email { get; set; }
    }
}
