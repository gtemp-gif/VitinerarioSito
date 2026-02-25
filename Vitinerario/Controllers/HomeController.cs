using Microsoft.AspNetCore.Mvc;

namespace Vitinerario.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Archive()
        {
            return View();
        }

        public IActionResult EventDetails()
        {
            return View();
        }

        public IActionResult Events()
        {
            return View();
        }

        public IActionResult StyleGuide()
        {
            return View();
        }

        public IActionResult Producers()
        {
            return View();
        }

        public IActionResult Article()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Podcast()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
