using LiveCharts;
using System;
using Prism.Mvvm;

namespace MZ.Domain.Models
{
    public class ReportImageFileModel : BindableBase
    {
        private DateTime _date;
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }

        private int _count;
        public int Count { get => _count; set => SetProperty(ref _count, value); }

    }

    public class ReportObjectDetectionModel : BindableBase
    {
        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private int _count;
        public int Count { get => _count; set => SetProperty(ref _count, value); }

        private string _color;
        public string Color { get => _color; set => SetProperty(ref _color, value); }

        private double _percent;
        public double Percent { get => _percent; set => SetProperty(ref _percent, value); }
    }

    public class ReportChartModel : BindableBase
    {
        private SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection { get => _seriesCollection; set => SetProperty(ref _seriesCollection, value); }

        private string[] _labels;
        public string[] Labels { get => _labels; set => SetProperty(ref _labels, value); }

        private Func<double, string> _formatter;
        public Func<double, string> Formatter { get => _formatter; set => SetProperty(ref _formatter, value); }

        private int _separator = 1;
        public int Separator
        {
            get => _separator;
            set
            {
                if (_separator < 1)
                {
                    _separator = 1;
                }
                SetProperty(ref _separator, value);
            }
        }

    }

}
