using LiveChartsCore;
using System;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView;

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
        private ObservableCollection<ISeries> _seriesCollection;
        public ObservableCollection<ISeries> SeriesCollection { get => _seriesCollection; set => SetProperty(ref _seriesCollection, value); }

        private Axis[] _xAxes;
        public Axis[] XAxes
        {
            get => _xAxes;
            set => SetProperty(ref _xAxes, value);
        }

        private Axis[] _yAxes;
        public Axis[] YAxes
        {
            get => _yAxes;
            set => SetProperty(ref _yAxes, value);
        }

    }

}
