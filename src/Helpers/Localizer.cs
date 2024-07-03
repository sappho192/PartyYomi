using Lepo.i18n;

namespace PartyYomi.Helpers
{
    public class Localizer
    {
        public static string getString(string key)
        {
            var localizationProvider = LocalizationProviderFactory.GetInstance();
            var currentCulture = localizationProvider.GetCulture();
            var localizationSet = localizationProvider.GetLocalizationSet(currentCulture.ToString());
            return localizationSet[key];
        }

        public static void ChangeLanguage(string languageCode)
        {
            var localizationProvider = LocalizationProviderFactory.GetInstance();
            localizationProvider.SetCulture(new(languageCode));
        }
    }
}
