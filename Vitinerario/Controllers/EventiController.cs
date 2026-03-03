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
                // Logic to save participation would go here (could also be replaced by an API call)

                TempData["SuccessMessage"] = "Richiesta inviata con successo! Ti contatteremo presto.";
                return RedirectToAction("Partecipa");
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
