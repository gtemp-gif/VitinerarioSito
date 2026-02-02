namespace Vitinerario.Models;

public class SectionViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string LinkUrl { get; set; } = string.Empty;
    public string LinkText { get; set; } = string.Empty;
    public bool IsImageLeft { get; set; } // Helper for layout
}
