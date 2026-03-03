using System.Threading;

namespace Vitinerario.Helpers
{
    public static class LanguageHelper
    {
        public static int GetCurrentLangId()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            return currentCulture.ToLower() == "it" ? 2 : 1;
        }
    }
}
