using System.Windows;
using ControlzEx.Theming;
using Microsoft.Win32;

namespace MZ.Resource
{

    public static class ThemeService
    {
        public static void SetTheme(string theme)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, theme);
        }

        public static void SetThemeInDatabase(string theme, string color = "Steel")
        {
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{color}");
        }

        public static string ChangeMode()
        {
            var theme = ThemeManager.Current.DetectTheme(Application.Current);
            string mode = theme.DisplayName.Contains("Dark") ? "Light" : "Dark";
            ThemeManager.Current.ChangeTheme(Application.Current, $"{mode}.Steel");
            return mode;
        }

        public static string GetSystemTheme()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value != null && value is int themeValue)
                        {
                            return themeValue == 0 ? "Dark" : "Light";
                        }
                    }
                }
            }
            catch
            {
                //AppLogger.Instance.Warning("Unknow Theme");
            }
            return "Light";
        }
    }
}
