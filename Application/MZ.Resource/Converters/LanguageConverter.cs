using System;
using System.Globalization;
using System.Windows.Data;

namespace MZ.Resource.Converters
{
    /// <summary>
    /// 리소스 키(string)를 다국어 번역 문자열로 변환
    /// </summary>
    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string resourceKey)
            {
                string translated = LanguageService.GetString(resourceKey);
                return string.IsNullOrEmpty(translated) ? resourceKey : translated;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
