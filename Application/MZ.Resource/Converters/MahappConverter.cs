using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    public class MahappForegroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Application.Current.TryFindResource("MahApps.Brushes.ThemeForeground");
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
