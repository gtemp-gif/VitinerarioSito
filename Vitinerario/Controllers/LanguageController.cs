using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;

namespace Vitinerario.Controllers
{
    public class LanguageController : Controller
    {
        [HttpGet]
        [HttpGet]
        public IActionResult ChangeLanguage(int langId, string returnUrl)
        {
            if (langId == 1 || langId == 2)
            {
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    Path = "/" // IMPORTANTE: assicura che il cookie sia leggibile da tutte le pagine
                };

                Response.Cookies.Append("UserLanguage", langId.ToString(), options);
            }
            // 2. LOGICA DI REINDIRIZZAMENTO PERSONALIZZATA
            // Verifichiamo se l'URL di provenienza contiene i path di dettaglio
            if (!string.IsNullOrEmpty(returnUrl))
            {
                // Se siamo nel dettaglio di un evento, rimanda alla lista eventi
                if (returnUrl.Contains("Home/EventDetails"))
                {
                    returnUrl = Url.Action("Events", "Home");
                }
                // Se siamo nel dettaglio di un articolo, rimanda all'archivio
                else if (returnUrl.Contains("Home/Article"))
                {
                    returnUrl = Url.Action("Archive", "Home");
                }
            }

            // 3. ESECUZIONE REDIRECT
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
