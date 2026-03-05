using Microsoft.AspNetCore.Mvc;
using Vitinerario.Models;
using Vitinerario.Services;
using Vitinerario.Helpers;
using System.IO;
using Vitinerario.Models.Dtos;

namespace Vitinerario.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IApiService _apiService;

        public HomeController(IWebHostEnvironment env, IApiService apiService)
        {
            _env = env;
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetEventsAsync();
            var articles = await _apiService.GetContentsByTypeAsync("blog", LanguageHelper.GetCurrentLangId());
            var news = await _apiService.GetContentsByTypeAsync("news", LanguageHelper.GetCurrentLangId());
            var podcasts = await _apiService.GetPodcastsAsync(LanguageHelper.GetCurrentLangId());

            // Taking top 3 for index display 
            ViewBag.LatestEvents = events?.Take(3).ToList() ?? new List<EventViewModel>();
            ViewBag.LatestArticles = articles?.Take(4).ToList() ?? new List<ContentDto>();
            ViewBag.LatestNews = news?.Take(4).ToList() ?? new List<ContentDto>();
            ViewBag.LatestPodcasts = podcasts?.Take(3).ToList() ?? new List<ContentDto>();
            return View();
        }

        public async Task<IActionResult> Archive()
        {
            int langId = LanguageHelper.GetCurrentLangId();
            var articles = await _apiService.GetContentsByTypeAsync("blog", langId);
            ViewBag.Articles = articles?.OrderByDescending(a => a.PublishDate).ToList() ?? new List<ContentDto>();
            return View();
        }

        [Route("Home/EventDetails/{id}")]
        public async Task<IActionResult> EventDetails(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Events");
            }

            int langId = LanguageHelper.GetCurrentLangId();
            var eventDto = await _apiService.GetEventById(id, langId);

            if (eventDto == null)
            {
                return NotFound();
            }

            return View(eventDto);
        }

        public async Task<IActionResult> Events()
        {
            var events = await _apiService.GetEventsAsync();
            
            return View(events ?? new List<EventViewModel>());

        }

        public IActionResult StyleGuide()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Producers()
        {
            return View(new ProducerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Producers(ProducerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Submitting producer request using the real API
                bool isSuccess = await _apiService.SubmitProducerAsync(model);

                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Richiesta inviata con successo! Ti contatteremo presto.";
                    return RedirectToAction(nameof(Producers));
                }
                else
                {
                    ModelState.AddModelError("", "Errore durante l'invio della richiesta al server. Riprova più tardi.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log exception
                ModelState.AddModelError("", "Si è verificato un errore durante l'invio della richiesta. Riprova più tardi.");
                return View(model);
            }
        }

        public IActionResult DownloadBrochure()
        {
            string filePath = Path.Combine(_env.WebRootPath, "Assets", "doc", "Brochure Vitinerario.pdf");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Il file richiesto non è disponibile.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", "Brochure Vitinerario.pdf");
        }

        public async Task<IActionResult> Article(int id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction("Archive");
            }

            int langId = LanguageHelper.GetCurrentLangId();

            // Recuperiamo il singolo articolo tramite il suo ID
            // Nota: Assicurati che il metodo nel servizio si chiami GetContentById (singolare)
            var article = await _apiService.GetContentById(id, langId);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Podcast()
        {
            int langId = LanguageHelper.GetCurrentLangId();
            var podcasts = await _apiService.GetPodcastsAsync(langId);
            return View(podcasts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
