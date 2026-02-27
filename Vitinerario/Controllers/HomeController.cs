using Microsoft.AspNetCore.Mvc;
using Vitinerario.Models;
using System.IO;
using Vitinerario.Services;

namespace Vitinerario.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public HomeController(IWebHostEnvironment env, IEmailService emailService, IConfiguration configuration)
        {
            _env = env;
            _emailService = emailService;
            _configuration = configuration;
        }

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

            // --- EMAIL SENDING LOGIC ---
            try
            {
                string subject = $"{model.RequestType} - Richiesta da Pagina Producers";
                string body = $@"
                    <h2>Nuova Richiesta da Pagina Producers</h2>
                    <p><strong>Azienda:</strong> {model.CompanyName}</p>
                    <p><strong>Referente:</strong> {model.ContactPerson}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Tipo Richiesta:</strong> {model.RequestType}</p>
                    <hr />
                    <p><strong>Messaggio:</strong></p>
                    <p>{model.Message}</p>
                ";

                string adminEmail = _configuration["AdminEmail"];
                await _emailService.SendEmailAsync(adminEmail, subject, body);

                TempData["SuccessMessage"] = "Richiesta inviata con successo! Ti contatteremo presto.";
                return RedirectToAction(nameof(Producers));
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

        public IActionResult Article()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Privacy()
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
