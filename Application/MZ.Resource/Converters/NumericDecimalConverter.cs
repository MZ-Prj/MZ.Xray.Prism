using System.Globalization;
using System;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    public class NumericDecimalConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return doubleValue.ToString("F2", culture);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;
            var cleaned = CleanText(text);

            if (double.TryParse(cleaned, NumberStyles.Any, culture, out double doubleResult))
            {
                return doubleResult;
            }
            else
            {
                return 0.0;
            }
        }

        private string CleanText(string input)
        {
            string filtered = "";
            foreach (char c in input)
            {
                if (char.IsDigit(c) || c == '-' || c == '.')
                    filtered += c;
            }

            int firstMinus = filtered.IndexOf('-');
            if (firstMinus >= 0)
            {
                filtered = "-" + filtered.Replace("-", string.Empty);
            }

            int firstDot = filtered.IndexOf('.');
            if (firstDot >= 0)
            {
                string beforeDot = filtered.Substring(0, firstDot + 1);
                string afterDot = filtered.Substring(firstDot + 1).Replace(".", string.Empty);
                filtered = beforeDot + afterDot;
            }

            return filtered;
        }
    }
}
