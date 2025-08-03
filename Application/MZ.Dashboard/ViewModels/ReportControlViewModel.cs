using MZ.DTO;
using MZ.Core;
using MZ.Util;
using MZ.Domain.Models;
using MZ.Infrastructure;
using MZ.Domain.Entities;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using Prism.Ioc;
using Prism.Commands;
using Microsoft.Win32;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MZ.Dashboard.ViewModels
{
    public class ReportControlViewModel : MZBindableBase
    {
        private readonly IDatabaseService _databaseService;

        #region Params
        private DateTime? _startSelectedDate = DateTime.Today;
        public DateTime? StartSelectedDate { get => _startSelectedDate; set => SetProperty(ref _startSelectedDate, value); }

        private DateTime? _endSelectedDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        public DateTime? EndSelectedDate
        {
            get => _endSelectedDate;
            set
            {
                if (value.HasValue)
                {
                    var endOfDay = value.Value.Date.AddDays(1).AddSeconds(-1);
                    SetProperty(ref _endSelectedDate, endOfDay);
                }
                else
                {
                    SetProperty(ref _endSelectedDate, null);
                }
            }
        }

        private ReportChartModel _objectDetectionChart = new();
        public ReportChartModel ObjectDetectionChart { get => _objectDetectionChart; set => SetProperty(ref _objectDetectionChart, value); }

        private ReportChartModel _imageFileChart = new();
        public ReportChartModel ImageFileChart { get => _imageFileChart; set => SetProperty(ref _imageFileChart, value); }

        private ObservableCollection<ReportObjectDetectionModel> _objectDetectionData;
        public ObservableCollection<ReportObjectDetectionModel> ObjectDetectionData { get => _objectDetectionData; set => SetProperty(ref _objectDetectionData, value); }

        private ObservableCollection<ReportImageFileModel> _imageFileData;
        public ObservableCollection<ReportImageFileModel> ImageFileData { get => _imageFileData; set => SetProperty(ref _imageFileData, value); }
        #endregion


        #region Command

        private DelegateCommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new DelegateCommand(MZAction.Wrapper(SearchButton));

        private DelegateCommand<object> _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new DelegateCommand<object>(MZAction.Wrapper<object>(SaveButton));

        #endregion

        public ReportControlViewModel(IContainerExtension container, IDatabaseService databaseService) : base(container)
        {
            _databaseService = databaseService;

            base.Initialize();
        }

        private async void SearchButton()
        {
            await UpdateReport();
        }

        private void SaveButton(object charts)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Save PDF",
                Filter = "PDF  (*.pdf)|*.pdf",
                DefaultExt = ".pdf"
            };

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                //todo pdf save
            }
        }

        private void GridObjectDetectionChart(ICollection<ImageEntity> images)
        {
            // grid predict result (live chart)

            if (images.Count == 0)
            {
                return;
            }

            ObjectDetectionChart = new();

            var model = images
               .SelectMany(image => image.ObjectDetections)
               .ToList();

            var groupedData = model
                .GroupBy(od => od.Name)
                .Select(g => new { Name = g.Key, Count = g.Count(), g.First().Color })
                .OrderByDescending(x => x.Count)
                .ToList();

            var totalCount = groupedData.Sum(g => g.Count);

            ObjectDetectionChart.Labels = [.. groupedData.Select(x => x.Name)];
            ObjectDetectionChart.SeriesCollection = [];

            foreach (var group in groupedData)
            {
                var brushConverter = new BrushConverter();
                var brush = (SolidColorBrush)brushConverter.ConvertFrom(group.Color);

                double percent = totalCount > 0 ? (group.Count / (double)totalCount) * 100 : 0;
                ObjectDetectionChart.SeriesCollection.Add(new RowSeries
                {
                    Values = new ChartValues<int> { group.Count },
                    Fill = brush,
                    DataLabels = true,
                    LabelPoint = point => $"{point.X:N0}",
                    LabelsPosition = BarLabelPosition.Top
                });
            }

            ObjectDetectionChart.Separator = (groupedData == null || groupedData.Count == 0) ? 1 : Math.Max(1, (int)(groupedData[0].Count / 10));
            ObjectDetectionChart.Formatter = value => value.ToString("N0");
        }

        private void GridObjectDetectionData(ICollection<ImageEntity> images)
        {
            // grid predict result (tabel)

            if (images.Count == 0)
            {
                return;
            }

            var model = images.SelectMany(image => image.ObjectDetections).ToList();

            var groupedData = model
                .GroupBy(det => det.Name)
                .Select(g => new ReportObjectDetectionModel
                {
                    Name = g.Key,
                    Count = g.Count(),
                    Color = g.First().Color
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            int totalCount = groupedData.Sum(item => item.Count);
            foreach (var item in groupedData)
            {
                item.Percent = totalCount > 0 ? (item.Count / (double)totalCount) * 100 : 0;
            }

            ObjectDetectionData = [.. groupedData];
        }

        private void GridImageFileChart(ICollection<ImageEntity> images)
        {
            // grid predict result (live chart)

            ImageFileChart = new();

            var model = images
                .GroupBy(image => image.CreateDate.Date)
                .Select(s => new { Date = s.Key, Count = s.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            ImageFileChart.Labels = [.. model.Select(x => x.Date.ToString("yyyy-MM-dd"))];
            var values = new ChartValues<int>(model.Select(x => x.Count));

            ImageFileChart.SeriesCollection =
            [
                new ColumnSeries
                {
                    Title = "Image Count",
                    Values = values
                }
            ];
            ImageFileChart.Separator = (int)(1);
            ImageFileChart.Formatter = value => value.ToString("N0");
        }

        private void GridImageFileData(ICollection<ImageEntity> images)
        {
            // grid predict result (tabel)
            var model = images
                .GroupBy(image => image.CreateDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            var groupedData = model.Select(x => new ReportImageFileModel
            {
                Date = x.Date,
                Count = x.Count
            }).ToList();

            int totalCount = groupedData.Sum(item => item.Count);

            ImageFileData = [.. groupedData];
        }

        public override async void InitializeModel()
        {
            await UpdateReport();
        }

        public async Task UpdateReport()
        {

            if (StartSelectedDate.HasValue && EndSelectedDate.HasValue)
            {
                var response = await _databaseService.Image.Load(new ReportImageLoadRequest(StartSelectedDate.Value, EndSelectedDate.Value));

                if (response.Success)
                {
                    ICollection<ImageEntity> images = response.Data;

                    GridImageFileChart(images);
                    GridImageFileData(images);

                    GridObjectDetectionChart(images);
                    GridObjectDetectionData(images);
                }

            }
        }
    }
}
