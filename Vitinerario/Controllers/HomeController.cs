using System.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Vitinerario.Models;
using Vitinerario.Services;

namespace Vitinerario.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContentService _contentService;

        public HomeController(ILogger<HomeController> logger, IContentService contentService)
        {
            _logger = logger;
            _contentService = contentService;
        }

        public IActionResult Index()
        {
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf?.RequestCulture.Culture.Name ?? "it";

            var model = _contentService.GetHomeContent(culture);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
