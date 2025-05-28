using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MZ.Resource.Converters
{

    public class PlaceholderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            string placeholder = parameter as string;

            return string.IsNullOrEmpty(text) ? placeholder : text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            string placeholder = parameter as string;

            return text == placeholder ? string.Empty : text;
        }
    }
}
