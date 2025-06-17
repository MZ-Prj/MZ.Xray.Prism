using System;
using System.Globalization;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    public class NumericIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString(culture);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return 0;
                }

                var filtered = "";
                foreach (char c in text)
                {
                    if (char.IsDigit(c) || c == '-')
                        filtered += c;
                }

                int firstMinus = filtered.IndexOf('-');
                if (firstMinus >= 0)
                {
                    filtered = "-" + filtered.Replace("-", string.Empty);
                }

                if (int.TryParse(filtered, NumberStyles.Integer, culture, out int result))
                {
                    return result;
                }
            }

            return 0;
        }
    }
}
