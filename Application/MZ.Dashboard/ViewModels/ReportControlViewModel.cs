using MZ.DTO;
using MZ.Core;
using MZ.Util;
using MZ.Resource;
using MZ.Xray.Engine;
using MZ.Domain.Models;
using MZ.Infrastructure;
using MZ.Domain.Entities;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using Prism.Ioc;
using Prism.Commands;
using Microsoft.Win32;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Kernel.Events;
using Axis = LiveChartsCore.SkiaSharpView.Axis;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// Report Control ViewModel : 현 수집된 데이터 분석(인공지능, 파일개수) 수행 
    /// </summary>
    public class ReportControlViewModel : MZBindableBase
    {
        #region Service
        private readonly IDatabaseService _databaseService;
        private readonly IXrayService _xrayService;

        public PDFProcesser PDF 
        {
            get => _xrayService.PDF;
            set => _xrayService.PDF = value;
        }

        #endregion

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


        #region Commands

        private DelegateCommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new DelegateCommand(MZAction.Wrapper(SearchButton));

        private DelegateCommand<object> _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new DelegateCommand<object>(MZAction.Wrapper<object>(SaveButton));

        #endregion

        public ReportControlViewModel(IContainerExtension container, IXrayService xrayService, IDatabaseService databaseService) : base(container)
        {
            _databaseService = databaseService;
            _xrayService = xrayService;


            base.Initialize();
        }

        public override async void InitializeModel()
        {
            await UpdateReport();
        }

        /// <summary>
        /// 분석(검색) 실행
        /// </summary>
        private async void SearchButton()
        {
            await UpdateReport();
        }

        /// <summary>
        /// PDF 저장 다이얼로그 및 저장 처리
        /// </summary>
        /// <param name="charts">object : 차트 오브젝트(Framework)</param>
        private async void SaveButton(object charts)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = LanguageService.GetString($"Lng{MZResourceNames.SavePDF}"),
                Filter = "PDF  (*.pdf)|*.pdf",
                DefaultExt = ".pdf"
            };

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                PDF.StartSelectedDate = StartSelectedDate.Value;
                PDF.EndSelectedDate = EndSelectedDate.Value;
                PDF.Username = _databaseService.User.CurrentUser().Data;

                if (StartSelectedDate.HasValue && EndSelectedDate.HasValue)
                {
                    var response = await _databaseService.Image.Load(new ReportImageLoadRequest(StartSelectedDate.Value, EndSelectedDate.Value));

                    if (response.Success)
                    {
                        ICollection<ImageEntity> images = response.Data;
                        PDF.Make(charts, saveFileDialog.FileName, "ObjectDetectionLiveChart", "ImageFileLiveChart", images);
                    }
                }
            }
        }

        /// <summary>
        /// 객체 탐지 차트 갱신
        /// </summary>
        /// <param name="images">ICollection<ImageEntity> </param>
        private void GridObjectDetectionChart(ICollection<ImageEntity> images)
        {
            if (images == null || images.Count == 0)
            {
                ObjectDetectionChart = new ReportChartModel();
                return;
            }
            // 데이터 수집
            var model = images
                .SelectMany(image => image.ObjectDetections)
                .ToList();

            var groupedData = model
                .GroupBy(od => od.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count(),
                    Color = g.First().Color,
                })
                .OrderByDescending(x => x.Count)
                .ToList();
            groupedData.Reverse();
            
            
            //Live Charts
            List<SolidColorPaint> paints = [];
            foreach (var g in groupedData)
            {
                if (!string.IsNullOrEmpty(g.Color))
                {
                    paints.Add(new SolidColorPaint(SKColor.Parse(g.Color)));
                }
                else
                {
                    paints.Add(new SolidColorPaint(SKColors.SkyBlue));
                }
            }

            var values = groupedData.Select(g => g.Count).ToArray();
            var labels = groupedData.Select(g => g.Name).ToArray();

            var series = new RowSeries<int>
            {
                Values = values,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Middle,
                DataLabelsFormatter = (point) =>
                {
                    var idx = point.Index;
                    if (idx >= 0 && idx < labels.Length)
                    {
                        return labels[idx];
                    }
                    return string.Empty;
                }
            }
            .OnPointMeasured(point =>
            {
                if (point.Visual is null)
                {
                    return;
                }
                point.Visual.Fill = paints[point.Index % paints.Count];
            });

            ObjectDetectionChart.SeriesCollection = [series];
            ObjectDetectionChart.XAxes =
            [
                new Axis
                {
                    Name = "Count",
                    Labeler = value => value.ToString("N0"),
                    NameTextSize = 12
                }
            ];
            ObjectDetectionChart.YAxes =
            [
                new Axis
                {
                    Name = "Object Name",
                    Labels = labels,
                    IsVisible = false
                }
            ];
        }

        /// <summary>
        /// 객체 탐지 데이터(테이블) 갱신
        /// </summary>
        /// <param name="images">ICollection<ImageEntity></param>
        private void GridObjectDetectionData(ICollection<ImageEntity> images)
        {
            if (images.Count == 0)
            {
                return;
            }

            // 데이터 수집
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

            // Data Grid
            ObjectDetectionData = [.. groupedData];
        }

        /// <summary>
        /// 이미지 파일 차트 갱신
        /// </summary>
        /// <param name="images">ICollection<ImageEntity></param>
        private void GridImageFileChart(ICollection<ImageEntity> images)
        {
            //Live Charts
            ImageFileChart = new();

            var model = images
                .GroupBy(image => image.CreateDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var minDate = model.Keys.Min();
            var maxDate = model.Keys.Max();

            var dateList = new List<DateTime>();
            for (var d = minDate; d <= maxDate; d = d.AddDays(1))
            {
                dateList.Add(d);
            }

            var labels = dateList.Select(x => x.ToString("yyyy-MM-dd")).ToArray();
            var values = dateList.Select(x => model.TryGetValue(x, out int value) ? (double)value : 0.0).ToArray();

            ImageFileChart.SeriesCollection =
            [
                new LineSeries<double>
                {
                    Values = values,
                }
            ];

            ImageFileChart.XAxes =
            [
                new Axis
                {
                    Labels = labels,
                }
            ];

            ImageFileChart.YAxes =
            [
                new Axis
                {
                }
            ];

        }

        /// <summary>
        /// 이미지 파일 데이터(테이블) 갱신
        /// </summary>
        /// <param name="images">ICollection<ImageEntity></param>
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


        /// <summary>
        /// 차트 및 데이터 갱신
        /// </summary>
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
