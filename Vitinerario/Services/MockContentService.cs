using Vitinerario.Models;

namespace Vitinerario.Services;

public class MockContentService : IContentService
{
    public HomeViewModel GetHomeContent(string culture)
    {
        var model = new HomeViewModel();
        bool isIt = culture.ToLower().StartsWith("it");

        // Section 1: Intro/Articles
        model.Sections.Add(new SectionViewModel
        {
            Title = "",
            Description = isIt
                ? "Pensieri sul vino, il cibo e su tutto ciò che riguarda il mondo enogastronomico."
                : "Thoughts on wine, food and everything related to the food and wine world.",
            LinkText = isIt ? "Leggi qui" : "Read here",
            LinkUrl = "/articoli2-articoli", // In a real app, use Url.Action or specific route
            ImageUrl = "https://files.supersite.aruba.it/media/31473_8557c1fdbb4e94fb2f3903ae05abf9fb5bff3678.svg", // Using the svg found in source
            IsImageLeft = false
        });

        // Section 2: App
        model.Sections.Add(new SectionViewModel
        {
            Title = "Vitinerario APP",
            Description = isIt
                ? "Vitinerario – Il tuo sommelier virtuale sempre con te!"
                : "Vitinerario – Your virtual sommelier always with you!",
            LinkText = isIt ? "Scopri di più sulla nostra App" : "Find out more about our App",
            LinkUrl = "/vitinerario-app",
            ImageUrl = "https://files.supersite.aruba.it/media/31473_2a7483fd2164654da002627bf37cf6da4dcd3538.png/v1/w_561,h_0/5ff4ecb8-c060-4cea-8e64-89c49198358d.png",
            IsImageLeft = true
        });

        // Section 3: Network
        model.Sections.Add(new SectionViewModel
        {
            Title = "VITINERARIO NETWORK",
            Description = "",
            LinkText = isIt ? "Scopri tutti i partner" : "Discover all partners",
            LinkUrl = "/partner",
            ImageUrl = "https://files.supersite.aruba.it/media/31473_c873934a6603e22a745d8acdac0ac08e83565ea7.svg",
            IsImageLeft = false
        });

        // Section 4: Contact
        model.Sections.Add(new SectionViewModel
        {
            Title = isIt ? "Contattaci per maggiori informazioni o per iscriverti alla nostra newsletter" : "Contact us for more information or to subscribe to our newsletter",
            Description = "",
            LinkText = isIt ? "Contattaci" : "Contact Us",
            LinkUrl = "/contact", // Assuming a contact page
            ImageUrl = "",
            IsImageLeft = true
        });

        return model;
    }
}
