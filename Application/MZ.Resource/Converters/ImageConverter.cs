using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;

namespace MZ.Resource.Converters
{
    /// <summary>
    /// 이미지 경로(string)를 ImageBrush로 변환
    /// </summary>
    public class ImagePathToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrEmpty(path))
            {
                try
                {
                    BitmapImage image = new(new Uri(path, UriKind.RelativeOrAbsolute));
                    return new ImageBrush(image);
                }
                catch
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }


    public class ImagePathToBrushMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var path = values[0]?.ToString();
            var fileName = values[1]?.ToString();
            string fullPath = Path.Combine(path, fileName);

            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            try
            {
                var uri = new Uri(fullPath, UriKind.RelativeOrAbsolute);
                return new BitmapImage(uri);
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
