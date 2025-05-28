using System.Globalization;
using System.Windows;
using System;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolean = value is bool;
            return boolean ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visibility = value is Visibility;
            return visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
