using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vitinerario.Models;
using Vitinerario.Services;

namespace Vitinerario.Controllers
{
    public class EventiController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public EventiController(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Partecipa()
        {
            ViewBag.Eventi = GetEventiList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Partecipa(PartecipaViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic to save participation
                string adminEmail = _configuration["AdminEmail"];
                string subject = $"Nuova partecipazione: {model.Evento}";
                string body = $@"
                    <h2>Nuova Partecipazione Evento</h2>
                    <p><strong>Nome:</strong> {model.Nome}</p>
                    <p><strong>Cognome:</strong> {model.Cognome}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Evento:</strong> {model.Evento}</p>
                ";

                await _emailService.SendEmailAsync(adminEmail, subject, body);

                TempData["SuccessMessage"] = "Richiesta inviata con successo! Ti contatteremo presto.";
                return RedirectToAction("Partecipa");
            }

            ViewBag.Eventi = GetEventiList();
            return View(model);
        }

        private List<SelectListItem> GetEventiList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Paris Fashion Week Gala", Text = "Paris Fashion Week Gala - 14 Jun" },
                new SelectListItem { Value = "Modernism Art Fair", Text = "Modernism Art Fair - 22 Jun" },
                new SelectListItem { Value = "Cannes Yachting Festival", Text = "Cannes Yachting Festival - 05 Jul" },
                new SelectListItem { Value = "Rooftop Jazz Sessions", Text = "Rooftop Jazz Sessions - 18 Jul" }
            };
        }
    }
}
