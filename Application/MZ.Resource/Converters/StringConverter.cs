using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MZ.Resource.Converters
{

    public class UppercaseConverter : IValueConverter
    {
        private static readonly Regex _regex = new("[-_]", RegexOptions.Compiled);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input)
            {
                var _input = _regex.Replace(input, string.Empty);
                return _input.ToUpper(culture);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
