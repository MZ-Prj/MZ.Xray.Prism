using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    /// <summary>
    /// 값이 비어 있으면 placeholder(매개변수)로 대체, 그렇지 않으면 원래 값을 반환
    /// </summary>
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
