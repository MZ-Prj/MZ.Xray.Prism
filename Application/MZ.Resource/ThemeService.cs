using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using Microsoft.Win32;
using MZ.Domain.Enums;
using MZ.Util;

namespace MZ.Resource
{
    /// <summary>
    /// 테마(라이트/다크 등) 관리 및 시스템 테마 감지, 리소스 브러시 조회 
    /// </summary>
    public static class ThemeService
    {
        /// <summary>
        /// 테마 정보
        /// </summary>
        public static ThemeRole CurrentTheme = ThemeRole.LightSteel;

        public static void Load(ThemeRole theme)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, MZEnum.GetName(theme));
            CurrentTheme = theme;
        }

        public static ThemeRole ChangeMode()
        {
            bool isDark = ThemeManager.Current.DetectTheme(Application.Current).DisplayName.Contains("Dark");
            ThemeRole theme = isDark ? ThemeRole.LightSteel : ThemeRole.DarkSteel;
            ThemeManager.Current.ChangeTheme(Application.Current, MZEnum.GetName(theme));
            CurrentTheme = theme;

            return theme;
        }

        public static ThemeRole GetSystemTheme()
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key != null)
                {
                    object value = key.GetValue("AppsUseLightTheme");
                    if (value != null && value is int themeValue)
                    {
                        return themeValue == 0 ? ThemeRole.DarkSteel : ThemeRole.LightSteel;
                    }
                }
            }
            catch { }

            return ThemeRole.LightSteel;
        }

        public static Brush GetResource(string resource)
        {
            Brush brush = Application.Current.TryFindResource(resource) as Brush;
            return brush;
        }
    }
}
