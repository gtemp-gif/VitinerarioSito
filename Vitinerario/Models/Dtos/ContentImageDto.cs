namespace Vitinerario.Models.Dtos
{
    public sealed class ContentImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Caption { get; set; }
        public int? Position { get; set; }
    }
}
