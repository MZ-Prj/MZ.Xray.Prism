using System;
using System.Globalization;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    public class NumericFloatConverter : IValueConverter
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
}
