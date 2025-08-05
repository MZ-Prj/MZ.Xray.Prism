using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    /// <summary>
    /// 경로 문자열에서 디렉토리 이름만 추출하여 반환
    /// </summary>
    public class LastestDirectoryConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input)
            {
                var directoryInfo = new DirectoryInfo(input);
                return directoryInfo.Name;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
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
