using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;

namespace ATLManager.Services
{
    public class LanguageService
    {
        private readonly IStringLocalizer _localizer;

        public LanguageService(IStringLocalizerFactory factory)
        {
            var type = typeof(ShareResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResource", assemblyName.Name);
        }

        public LocalizedString GetKey(string key)
        {
            return _localizer[key];
        }

        public string GetCurrentLanguage()
        {
            // Example implementation - replace with your own code
            var currentCulture = CultureInfo.CurrentCulture;
            var currentLanguage = currentCulture.TwoLetterISOLanguageName;

            return currentLanguage;
        }


    }
}
