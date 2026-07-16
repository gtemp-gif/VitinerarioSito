namespace Vitinerario.Models.Dtos
{
    public class EventNeedDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}