using LiveChartsCore;
using System;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView;

namespace MZ.Domain.Models
{
    /// <summary>
    /// 리포트(통계) - 날짜별 이미지 파일 수 집계 모델
    /// 
    /// - 그리드/차트 UI에 바인딩하여 일자별 이미지 파일 개수 시각화에 활용
    /// </summary>
    public class ReportImageFileModel : BindableBase
    {
        /// <summary>
        /// 기준 날짜(yyyy-MM-dd )
        /// </summary>
        private DateTime _date;
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }

        /// <summary>
        /// 해당 날짜에 수집/분석된 이미지 파일 개수
        /// </summary>
        private int _count;
        public int Count { get => _count; set => SetProperty(ref _count, value); }

    }

    /// <summary>
    /// 리포트(통계) - 객체 검출별(카테고리별) 집계 모델
    /// 
    /// - AI/YOLO 등에서 객체 검출 통계, 차트/그리드에 활용
    /// </summary>
    public class ReportObjectDetectionModel : BindableBase
    {
        /// <summary>
        /// 검출 객체 이름(카테고리명)
        /// </summary>
        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        /// <summary>
        /// 해당 객체가 검출된 횟수(Count)
        /// </summary>
        private int _count;
        public int Count { get => _count; set => SetProperty(ref _count, value); }

        /// <summary>
        /// 카테고리별 색상(HEX)정보
        /// </summary>
        private string _color;
        public string Color { get => _color; set => SetProperty(ref _color, value); }

        /// <summary>
        /// 전체 중 비율(%)
        /// </summary>
        private double _percent;
        public double Percent { get => _percent; set => SetProperty(ref _percent, value); }
    }

    /// <summary>
    /// LiveCharts 차트 바인딩용 모델
    /// 
    /// - X축/Y축/시리즈 데이터 등을 통합 관리
    /// </summary>
    public class ReportChartModel : BindableBase
    {
        /// <summary>
        /// 차트 시리즈 데이터 집합 (예: Bar, Row 등)
        /// </summary>
        private ObservableCollection<ISeries> _seriesCollection;
        public ObservableCollection<ISeries> SeriesCollection { get => _seriesCollection; set => SetProperty(ref _seriesCollection, value); }

        /// <summary>
        /// X축 정보 (라벨, 스케일 등)
        /// </summary>
        private Axis[] _xAxes;
        public Axis[] XAxes
        {
            get => _xAxes;
            set => SetProperty(ref _xAxes, value);
        }

        /// <summary>
        /// Y축 정보 (라벨, 스케일 등)
        /// </summary>
        private Axis[] _yAxes;
        public Axis[] YAxes
        {
            get => _yAxes;
            set => SetProperty(ref _yAxes, value);
        }

    }

}
