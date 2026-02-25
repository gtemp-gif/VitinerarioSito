using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vitinerario.Models;

namespace Vitinerario.Controllers
{
    public class EventiController : Controller
    {
        [HttpGet]
        public IActionResult Partecipa()
        {
            ViewBag.Eventi = GetEventiList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Partecipa(PartecipaViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic to save participation would go here

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
