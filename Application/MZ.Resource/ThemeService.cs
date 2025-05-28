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

        public static string ChangeMode()
        {
            var theme = ThemeManager.Current.DetectTheme(Application.Current);
            string mode = theme.DisplayName.Contains("Dark") ? "Light" : "Dark";
            ThemeManager.Current.ChangeTheme(Application.Current, $"{mode}.Steel");
            return mode;
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
