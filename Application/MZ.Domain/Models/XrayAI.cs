using MZ.Domain.Interfaces;
using Prism.Mvvm;
using System;

namespace MZ.Domain.Models
{
    /// <summary>
    /// 객체 검출 옵션
    /// </summary>
    public class ObjectDetectionOptionModel : BindableBase
    {
        /// <summary>
        /// 신뢰도 임계값(0~1, default: 0.75)
        /// </summary>
        private double _confidence = 0.75;
        public double Confidence { get => _confidence; set => SetProperty(ref _confidence, value); }

        /// <summary>
        /// IoU 임계값(0~1, default: 0.95)
        /// </summary>
        private double _iou = 0.95;
        public double IoU { get => _iou; set => SetProperty(ref _iou, value); }

        /// <summary>
        /// X축 좌표 변환 및 리사이즈 보정계수
        /// </summary>
        private double _scaleX = 1.0;
        public double ScaleX { get => _scaleX; set => SetProperty(ref _scaleX, value); }

        /// <summary>
        /// Y축 좌표 변환 및 리사이즈 보정계수
        /// </summary>
        private double _scaleY = 1.0;
        public double ScaleY { get => _scaleY; set => SetProperty(ref _scaleY, value); }
    }

    /// <summary>
    /// AI 객체 검출 카테고리(클래스/라벨) 정보 모델
    /// 
    /// - AI 모델이 구분하는 객체 클래스(라벨) 정보
    /// - 색상, 활성화 여부 등 포함
    /// </summary>
    public class CategoryModel : BindableBase, ICategory
    {
        /// <summary>
        /// 고유 번호
        /// </summary>
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        /// <summary>
        /// 분류 인덱스
        /// </summary>
        private int _index;
        public int Index { get => _index; set => SetProperty(ref _index, value); }

        /// <summary>
        /// 분류명
        /// </summary>
        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        /// <summary>
        /// 색상(HEX)
        /// </summary>
        private string _color;
        public string Color { get => _color; set => SetProperty(ref _color, value); }

        /// <summary>
        /// 분류 활성화(사용/미사용)
        /// </summary>
        private bool _isUsing;
        public bool IsUsing { get => _isUsing; set => SetProperty(ref _isUsing, value); }

        /// <summary>
        /// 신뢰도(임계값, 0.0~1.0)
        /// </summary>
        private double _confidence;
        public double Confidence { get => _confidence; set => SetProperty(ref _confidence, value); }
    }

    /// <summary>
    /// 객체 검출(Detection) 결과 바인딩용 모델
    /// 
    /// - AI 분석 후, 이미지 내 객체별 검출 결과
    /// - 바운딩박스 좌표, 클래스명, 신뢰도 등 포함
    /// </summary>
    public class ObjectDetectionModel : BindableBase, IObjectDetection
    {
        /// <summary>
        /// 검출 결과 인덱스
        /// </summary>
        private int _index;
        public int Index { get => _index; set => SetProperty(ref _index, value); }
        /// <summary>
        /// 검출 클래스명
        /// </summary>
        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        /// <summary>
        /// 바운딩박스 컬러(HEX)
        /// </summary>
        private string _color;
        public string Color { get => _color; set => SetProperty(ref _color, value); }
        /// <summary>
        /// 검출 신뢰도(Confidence)
        /// </summary>
        private double _confidence;
        public double Confidence { get => _confidence; set => SetProperty(ref _confidence, value); }
        /// <summary>
        /// X
        /// </summary>
        private double _x;
        public double X { get => _x; set => SetProperty(ref _x, value); }
        /// <summary>
        /// Y
        /// </summary>
        private double _y;
        public double Y { get => _y; set => SetProperty(ref _y, value); }
        /// <summary>
        /// Width
        /// </summary>
        private double _width;
        public double Width { get => _width; set => SetProperty(ref _width, value); }
        /// <summary>
        /// Height
        /// </summary>
        private double _height;
        public double Height { get => _height; set => SetProperty(ref _height, value); }
        /// <summary>
        /// 검출 생성 일시(분석 수행 시각)
        /// </summary>
        private DateTime _createDate;
        public DateTime CreateDate { get => _createDate; set => SetProperty(ref _createDate, value); }

        //UI
        /// <summary>
        /// 내부 식별용 Id
        /// </summary>
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        /// <summary>
        /// 표시/숨김 제어
        /// </summary>
        private bool _isVisibility = true;
        public bool IsVisibility { get => _isVisibility; set => SetProperty(ref _isVisibility, value); }

        /// <summary>
        /// 깜빡임 효과용
        /// </summary>
        private bool _isBlink;
        public bool IsBlink { get => _isBlink; set => SetProperty(ref _isBlink, value); }
        /// <summary>
        /// UI 바운딩 박스의 X축 여백값(자르는 용도)
        /// </summary>
        private double _offsetX = 0;
        public double OffsetX { get => _offsetX; set => SetProperty(ref _offsetX, value); }

    }
}
