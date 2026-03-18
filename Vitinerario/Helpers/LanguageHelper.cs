using Microsoft.AspNetCore.Http;
using System.Threading;

namespace Vitinerario.Helpers
{
    public static class LanguageHelper
    {
        public static int GetCurrentLangId(HttpContext context = null)
        {
            if (context != null)
            {
                if (context.Request.Cookies.TryGetValue("UserLanguage", out string langValue) && int.TryParse(langValue, out int langId))
                {
                    return langId;
                }
            }

            // Fallback
            return 2; // Default to IT (2)
        }
    }
}
