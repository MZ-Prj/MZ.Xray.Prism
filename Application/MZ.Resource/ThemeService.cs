using System.Windows;
using ControlzEx.Theming;
using Microsoft.Win32;
using MZ.Domain.Enums;
using MZ.Util;

namespace MZ.Resource
{
    public static class ThemeService
    {
        public static void Load(ThemeRole theme)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, MZEnum.GetName(theme));
        }

        public static ThemeRole ChangeMode()
        {
            bool isDark = ThemeManager.Current.DetectTheme(Application.Current).DisplayName.Contains("Dark");
            ThemeRole theme = isDark ? ThemeRole.LightSteel : ThemeRole.DarkSteel;
            ThemeManager.Current.ChangeTheme(Application.Current, MZEnum.GetName(theme));
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

    }
}
