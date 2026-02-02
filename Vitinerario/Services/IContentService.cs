using Vitinerario.Models;

namespace Vitinerario.Services;

public interface IContentService
{
    HomeViewModel GetHomeContent(string culture);
}
