using MZ.Domain.Enums;
using MZ.Util;
using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MZ.Resource
{
    /// <summary>
    /// 다국어 문자열 관리 및 런타임 언어 변경 기능을 제공
    /// </summary>
    public static class LanguageService
    {
        public readonly static string Key = "Lng";
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

        public static void Load()
        {
            Load(MZEnum.GetName(CurrentLanguage));
        }

        public static void Load(string culture)
        {
            Load(new CultureInfo(culture));
        }

        public static async Task LoadAsync(string culture)
        {
            await Task.Run(() => Load(culture)); 
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

        /// <summary>
        /// 전체 리소스 로드 후 갱신
        /// </summary>
        public static string GetString(string key)
        {
            if (CultureInfo.CurrentUICulture.Name != MZEnum.GetName(CurrentLanguage))
            {
                Load(MZEnum.GetName(CurrentLanguage));
            }
            return resourceManager.GetString(key);
        }

        /// <summary>
        /// 현제 키에대한 부분만 갱신
        /// </summary>
        public static string GetStringOnlyKey(string key)
        {
            return resourceManager.GetString(key);
        }

        /// <summary>
        /// 현제 언어 반환
        /// </summary>
        public static string GetCurrentLanguage()
        {
            return CultureInfo.CurrentUICulture.Name;
        }

        /// <summary>
        /// 현제 언어 반환 (enum)
        /// </summary>
        public static LanguageRole? GetCurrentLanguageRole()
        {
            string code = GetCurrentLanguage();
            return MZEnum.Get<LanguageRole>(code);
        }

        /// <summary>
        /// 시스템 언어 반환
        /// </summary>
        public static string GetSystemLanguage()
        {
            CultureInfo systemCulture = CultureInfo.InstalledUICulture;
            return $"{systemCulture.Name}";
        }
    }
}
