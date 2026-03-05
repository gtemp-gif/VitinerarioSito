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
            int langId = LanguageHelper.GetCurrentLangId(HttpContext);
            var events = await _apiService.GetEventsAsync(langId);
            events = events?.Where(e => e.IsOnline).ToList(); // Filtra solo eventi futuri e ordina per data
            var articles = await _apiService.GetContentsByTypeAsync("blog", langId);
            articles = articles?.Where(a => a.IsPublished).ToList();
            var news = await _apiService.GetContentsByTypeAsync("news", langId);
            news = news?.Where(n => n.IsPublished).ToList();
            var podcasts = await _apiService.GetPodcastsAsync(langId);

            // Taking top 3 for index display 
            ViewBag.LatestEvents = events?.Take(3).ToList() ?? new List<EventDto>();
            ViewBag.LatestArticles = articles?.Take(3).ToList() ?? new List<ContentDto>();
            ViewBag.LatestNews = news?.Take(4).ToList() ?? new List<ContentDto>();
            ViewBag.LatestPodcasts = podcasts?.Take(3).ToList() ?? new List<ContentDto>();
            return View();
        }

        public async Task<IActionResult> Archive()
        {
            int langId = LanguageHelper.GetCurrentLangId(HttpContext);
            var articles = await _apiService.GetContentsByTypeAsync("blog", langId);
            articles = articles?.Where(a => a.IsPublished).ToList(); // Filtra solo gli articoli pubblicati
            ViewBag.Articles = articles?.OrderByDescending(a => a.PublishDate).ToList() ?? new List<ContentDto>();

            return View();
        }

        //public async Task<IActionResult> Archive(int page = 1)
        //{
        //    int pageSize = 10;
        //    var allArticles = await _apiService.GetContentsByTypeAsync("blog", langId); // Prendi tutti

        //    int totalItems = allArticles.Count;
        //    int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        //    // Filtra gli articoli per la pagina corrente
        //    var pagedArticles = allArticles
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    ViewBag.Articles = pagedArticles;
        //    ViewBag.CurrentPage = page;
        //    ViewBag.TotalPages = totalPages;
        //    ViewBag.TotalItems = totalItems;

        //    return View();
        //}

        [Route("Home/EventDetails/{id}")]
        public async Task<IActionResult> EventDetails(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Events");
            }

            int langId = LanguageHelper.GetCurrentLangId(HttpContext);
            var eventDto = await _apiService.GetEventById(id, langId);

            if (eventDto == null)
            {
                return NotFound();
            }

            return View(eventDto);
        }

        public async Task<IActionResult> Events()
        {
            int langId = LanguageHelper.GetCurrentLangId(HttpContext);
            var events = await _apiService.GetEventsAsync(langId);
            events = events?.Where(e =>  e.IsOnline).ToList(); 
            return View(events ?? new List<EventDto>());

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

            int langId = LanguageHelper.GetCurrentLangId(HttpContext);

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
            int langId = LanguageHelper.GetCurrentLangId(HttpContext);
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
