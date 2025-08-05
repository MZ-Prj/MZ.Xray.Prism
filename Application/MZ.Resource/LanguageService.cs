using MZ.Domain.Enums;
using MZ.Util;
using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows;

namespace MZ.Resource
{
    /// <summary>
    /// 다국어 문자열 관리 및 런타임 언어 변경 기능을 제공
    /// </summary>
    public static class LanguageService
    {
        /// <summary>
        /// 언어 정보 
        /// </summary>
        public static LanguageRole CurrentLanguage = LanguageRole.EnUS; 

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
            
            CurrentLanguage = MZEnum.Get<LanguageRole>(culture.Name) ?? LanguageRole.KoKR;

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

        public static string GetCurrentLanguage()
        {
            return CultureInfo.CurrentUICulture.Name;
        }

        public static LanguageRole? GetCurrentLanguageRole()
        {
            string code = CultureInfo.CurrentUICulture.Name;
            return MZEnum.Get<LanguageRole>(code);
        }

        public static string GetSystemLanguage()
        {
            CultureInfo systemCulture = CultureInfo.InstalledUICulture;
            return $"{systemCulture.Name}";
        }
    }
}
