using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Vitinerario.Controllers
{
    public class LanguageController : Controller
    {
        [HttpGet]
        public IActionResult ChangeLanguage(int langId, string returnUrl)
        {
            if (langId == 1 || langId == 2)
            {
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true, // Necessario per GDPR (se considerato essenziale)
                    HttpOnly = true,
                    Secure = Request.IsHttps
                };

                Response.Cookies.Append("UserLanguage", langId.ToString(), options);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
