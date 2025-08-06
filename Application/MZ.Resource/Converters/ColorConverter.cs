using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MZ.Resource.Converters
{
    /// <summary>
    /// 배경색의 명도(밝기)에 따라 검정/흰색 대비 색상을 반환
    /// </summary>
    public class ContrastColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color backgroundColor;

            if (value is SolidColorBrush brush)
            {
                backgroundColor = brush.Color;
            }
            else if (value is Color color)
            {
                backgroundColor = color;
            }
            else if (value is string hexString)
            {
                backgroundColor = ParseHexColor(hexString);
                if (backgroundColor == Colors.Transparent)
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }

            double brightness = GetPerceivedBrightness(backgroundColor);
            return brightness >= 128 ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color backgroundColor;

            if (value is SolidColorBrush brush)
            {
                backgroundColor = brush.Color;
            }
            else if (value is Color color)
            {
                backgroundColor = color;
            }
            else if (value is string hexString)
            {
                backgroundColor = ParseHexColor(hexString);
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }

            double brightness = GetPerceivedBrightness(backgroundColor);
            return brightness >= 196 ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
        }


        private double GetPerceivedBrightness(Color color)
        {
            return (0.299 * color.R) + (0.587 * color.G) + (0.114 * color.B);
        }

        private Color ParseHexColor(string hex)
        {
            try
            {
                if (!hex.StartsWith("#"))
                {
                    hex = "#" + hex;
                }

                if (hex.Length == 4)
                {
                    hex = "#" + new string(hex[1], 2) + new string(hex[2], 2) + new string(hex[3], 2);
                }

                return (Color)ColorConverter.ConvertFromString(hex);
            }
            catch
            {
                return Colors.Transparent;
            }
        }
    }

}
