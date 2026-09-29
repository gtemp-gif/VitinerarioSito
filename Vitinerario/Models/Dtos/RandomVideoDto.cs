namespace Vitinerario.Models.Dtos
{
    public class RandomVideoDto
    {
        public bool Success { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
    }
}