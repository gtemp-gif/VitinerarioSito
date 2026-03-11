using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IO;
using Vitinerario.Helpers;
using Vitinerario.Models;
using Vitinerario.Models.Dtos;
using Vitinerario.Models.Settings;
using Vitinerario.Services;

namespace Vitinerario.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IApiService _apiService;
        private readonly IEmailService _emailService;
        private readonly MailSettings _mailsettings;
        private readonly IConfiguration _configuration;
        public MailHelper.MailHelper _mailHelper { get; set; }
        public HomeController(IWebHostEnvironment env, IApiService apiService, IEmailService emailService, IConfiguration configuration)
        {
            _env = env;
            _apiService = apiService;
            _emailService = emailService;
            _configuration = configuration;
            _mailsettings = configuration.GetSection("MailSettings").Get<MailSettings>();

          
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
            // Recupero i partner e filtro solo quelli attivi (IsActive == true)
            var partners = await _apiService.GetPartnersAsync();
            ViewBag.Partners = partners?.Where(p => p.IsActive).ToList() ?? new List<PartnerDto>();
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
        [Route("privacy-policy")]
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


        public IActionResult CookiePolicy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContactForm(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ContactError"] = "Errore nella compilazione del modulo di contatto. Riprova.";
                return RedirectToAction("Index", "Home", null, "contact-section");
            }

            try
            {
                bool isSuccess =  await _emailService.SendContactEmailAsync(model); //SendEmail(model.Email ,model.Message );

                if (isSuccess)
                {
                    TempData["ContactSuccess"] = "Messaggio inviato con successo! Ti contatteremo presto.";
                }
                else
                {
                    TempData["ContactError"] = "Errore durante l'invio del messaggio. Riprova più tardi.";
                }
            }
            catch (Exception)
            {
                TempData["ContactError"] = "Si è verificato un errore imprevisto. Riprova più tardi.";
            }

            return RedirectToAction("Index", "Home", null, "contact-section");
        }

        [HttpPost]

        public bool SendEmail(string email, string request)

        {

            if (string.IsNullOrEmpty(email))

            {
                return false;//Json(new { success = false, message = "Email is required." });

            }

            try
            {
                MailSettings mail = _mailsettings;
                _mailHelper = new MailHelper.MailHelper
                {
                    FromEmail = mail.Mail,
                    FromEmailPwd = mail.Password,
                    Host = mail.Host,
                    Port = mail.Port,
                    EnableSSL = false,
                    SenderName = mail.SenderName
                };

                request = "Nuova richiesta utente da " + email + "  <br> <br> " + request;

                _mailHelper.SendEmail(mail.Mail, "Nuova richiesta utente", request);

                return true; //Json(new { success = true, message = "Email sent successfully." });

            }
            catch (Exception ex)

            {

                return false;//Json(new { success = false, message = $"Error: {ex.Message}" });

            }

        }

    }
}
