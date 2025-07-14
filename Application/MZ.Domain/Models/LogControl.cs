using System;
using System.Windows.Media;

namespace MZ.Domain.Models
{
    public class LogComboboxModel
    {
        public string Text { get; set; }
        public SolidColorBrush ColorBrush { get; set; }
    }

    public class LogControlModel
    {
        public DateTime Date { get; set; }
        public string LogLevel { get; set; }
        public int LineNumber { get; set; }
        public string Message { get; set; }
        public SolidColorBrush ColorBrush { get; set; }
    }
}
