using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vitinerario.Models;
using Vitinerario.Services;

namespace Vitinerario.Controllers
{
    public class EventiController : Controller
    {
        private readonly IApiService _apiService;

        public EventiController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Partecipa()
        {
            ViewBag.Eventi = await GetEventiListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Partecipa(PartecipaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isSuccess = await _apiService.SubmitPartecipaAsync(model);

                    if (isSuccess)
                    {
                        TempData["SuccessMessage"] = "Richiesta inviata con successo! Ti contatteremo presto.";
                        return RedirectToAction("Partecipa");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Errore durante l'invio della richiesta. Riprova più tardi.");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Si è verificato un errore di rete. Riprova.");
                }
            }

            ViewBag.Eventi = await GetEventiListAsync();
            return View(model);
        }

        private async Task<List<SelectListItem>> GetEventiListAsync()
        {
            var events = await _apiService.GetEventsAsync();

            if (events == null || !events.Any())
            {
                // Fallback or empty if API fails/returns nothing
                return new List<SelectListItem>();
            }

            return events.Select(e => new SelectListItem
            {
                Value = string.IsNullOrEmpty(e.Id) ? e.Title : e.Id,
                Text = $"{e.Title} - {e.Date:dd MMM}"
            }).ToList();
        }
    }
}
