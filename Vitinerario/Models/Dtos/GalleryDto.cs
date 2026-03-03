namespace Vitinerario.Models.Dtos
{
    public sealed class GalleryDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<PhotoDto> Photos { get; set; } = new();
    }
}
