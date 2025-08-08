using System;
using System.Globalization;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    public class CenterToolTipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2
               && values[0] is double targetWidth
               && values[1] is double tooltipWidth)
            {
                return (targetWidth - tooltipWidth) / 2;
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
