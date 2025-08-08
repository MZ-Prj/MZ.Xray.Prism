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
    /// 문자를 숫자로 변환 및 입력값 필터링 지원(double ↔ string)
    /// </summary>
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
                {
                    filtered += c;
                }
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


    /// <summary>
    /// 자료형 반환 (enum <-> float)
    /// </summary>
    public class NumericEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                return (float)System.Convert.ToInt32(enumValue);
            }
            return 0f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float floatValue)
            {
                return Enum.ToObject(targetType, (int)floatValue);
            }
            return Binding.DoNothing;
        }
    }


    /// <summary>
    /// 자료형 반환 (string <-> int)
    /// </summary>
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
