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
            var partners = await _apiService.GetPartnersAsync();
            ViewBag.Partners = partners?.Where(p => p.IsActive).OrderBy(p => p.Description).ToList() ?? new List<PartnerDto>();
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

            // --- LOGICA DI SMISTAMENTO VISTE ---
            // 3 = On Tour (IT) | 4 = On Tour (EN)
            if (eventDto.CategoryId == 3 || eventDto.CategoryId == 4)
            {
                var viewModel = new Vitinerario.Models.TravelEventViewModel { Event = eventDto };

                try
                {
                    var tripDto = await _apiService.GetTripByEventIdAsync(id);

                    if (tripDto != null)
                    {
                        viewModel.Trip = tripDto;
                        viewModel.Musts = await _apiService.GetTripMustsAsync(tripDto.Id);
                        viewModel.Stays = await _apiService.GetStaysAsync(tripDto.Id);

                        var days = await _apiService.GetItineraryDaysAsync(tripDto.Id);
                        foreach (var day in days.OrderBy(d => d.DayNumber))
                        {
                            var stops = await _apiService.GetItineraryStopsAsync(day.Id);
                            viewModel.Itinerary.Add(new Vitinerario.Models.FullItineraryDay
                            {
                                Day = day,
                                Stops = stops.OrderBy(s => s.OrderIndex).ToList()
                            });
                        }
                    }
                    // 1. Carica le varianti di prezzo (se previste)
                    if (eventDto.HasVariantPrice)
                    {
                        viewModel.VariantPrices = await _apiService.GetVariantPricesAsync(id);
                    }

                    // 2. Carica le "Informazioni Essenziali" (se previste)
                    if (eventDto.HasNeeds)
                    {
                        viewModel.EventNeeds = await _apiService.GetEventNeedsAsync(id);
                    }
                }
                catch (Exception ex)
                {
                    // Mettendo un breakpoint qui, puoi ispezionare l'errore 'ex.Message'
                    Console.WriteLine($"ERRORE API VIAGGIO: {ex.Message}");
                }

                return View("TravelEventDetails", viewModel);
            }
            // Per tutti gli altri eventi (Live 1/2, Art 5/6), carichiamo la vista classica
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
        public async Task<IActionResult> SubmitContactForm(ContactFormViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["ContactError"] = "Errore nella compilazione del modulo di contatto. Riprova.";
                // Se c'è un returnUrl usiamo quello, altrimenti torniamo alla Home
                if (!string.IsNullOrEmpty(returnUrl)) return LocalRedirect($"{returnUrl}#contact-section");
                return RedirectToAction("Index", "Home", null, "contact-section");
            }

            try
            {
                bool isSuccess = await _emailService.SendContactEmailAsync(model);

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

            // Se c'è un returnUrl usiamo quello, altrimenti torniamo alla Home
            if (!string.IsNullOrEmpty(returnUrl)) return LocalRedirect($"{returnUrl}#contact-section");

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


        public IActionResult TravelEventDetails(int id)
        {
           return View();
        }
        [Route("Home/Partners")]
        public async Task<IActionResult> Partners()
        {
            // Ottengo la lingua corrente (es. per future traduzioni)
            int langId = LanguageHelper.GetCurrentLangId(HttpContext);

            // Uso il tuo servizio API per ottenere tutti i partner
            var partners = await _apiService.GetPartnersAsync();

            // Filtriamo solo quelli attivi e LI ORDINIAMO ALFABETICAMENTE per il Nome (Description)
            // Aggiungiamo Trim() per prevenire spazi nascosti che sfalsano l'alfabeto
            var activePartners = partners?
                .Where(p => p.IsActive)
                .OrderBy(p => (p.Description ?? "").Trim())
                .ToList() ?? new List<PartnerDto>();

            // Passiamo la lista alla view tramite ViewBag
            ViewBag.Partners = activePartners;

            return View();
        }
    }
}
