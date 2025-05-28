using System;
using System.Globalization;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    public class PasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string password = value as string;
            return string.IsNullOrEmpty(password) ? string.Empty : new string('•', password.Length);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
