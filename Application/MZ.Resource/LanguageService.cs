using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows;

namespace MZ.Resource
{
    public static class LanguageService
    {
        private static readonly ResourceManager resourceManager;
        public static event EventHandler LanguageChanged;
        static LanguageService()
        {
            resourceManager = new ResourceManager("MZ.Resource.Languages.Resources", typeof(LanguageService).Assembly);
        }

        public static void Load(string culture)
        {
            Load(new CultureInfo(culture));
        }

        public static void Load(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            foreach (var entry in resourceSet.Cast<System.Collections.DictionaryEntry>())
            {
                Application.Current.Resources[entry.Key.ToString()] = entry.Value;
            }
            LanguageChanged?.Invoke(null, EventArgs.Empty);

        }

        public static string GetString(string key)
        {
            return resourceManager.GetString(key);
        }

        public static string GetCurrentLanguageCode()
        {
            return CultureInfo.CurrentUICulture.Name;
        }

        public static string GetSystemLanguage()
        {
            CultureInfo systemCulture = CultureInfo.InstalledUICulture;
            return $"{systemCulture.Name}";
        }
    }
}
