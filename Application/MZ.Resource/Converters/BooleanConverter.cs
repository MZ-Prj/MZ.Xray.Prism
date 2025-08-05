using System;
using System.Globalization;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    /// <summary>
    /// bool 값을 반대로 변환 (true <-> false)
    /// </summary>
    public class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool check)
            {
                return !check;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool check)
            {
                return !check;
            }

            return false;
        }
    }
}
