namespace Vitinerario.Models.Dtos
{
    public class PartnerDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}