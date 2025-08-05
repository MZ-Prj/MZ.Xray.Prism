using System;
using MZ.Domain.Interfaces;
using MZ.Domain.Enums;
using OpenCvSharp;
using Prism.Mvvm;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace MZ.Domain.Models
{
    /// <summary>
    /// 필터(Filter) 파라미터 정보 바인딩용 모델
    /// 
    /// - UI 조작/영상 처리(줌, 밝기, 색상모드 등)
    /// </summary>
    public class FilterModel : BindableBase, IFilter
    {
        /// <summary>
        /// Zoom 비율
        /// </summary>
        private float _zoom = 1.0f;
        public float Zoom { get => _zoom; set => SetProperty(ref _zoom, Math.Clamp(value, 1.0f, 5.0f)); }
        /// <summary>
        /// 선명도
        /// </summary>
        private float _sharpness = 0.0f;
        public float Sharpness { get => _sharpness; set => SetProperty(ref _sharpness, value); }
        /// <summary>
        /// 밝기
        /// </summary>
        private float _brightness = 0.0f;
        public float Brightness { get => _brightness; set => SetProperty(ref _brightness, Math.Clamp(value, 0.0f, 1.0f)); }
        /// <summary>
        /// 명암
        /// </summary>
        private float _contrast = 2.0f;
        public float Contrast { get => _contrast; set => SetProperty(ref _contrast, Math.Clamp(value, 0.0f, 5.0f)); }
        /// <summary>
        /// HLSL의 Shader에 들어갈 이미지 크기 Size
        /// </summary>
        private System.Windows.Size _size = new(1, 1);
        public System.Windows.Size Size { get => _size; set => SetProperty(ref _size, value); }
        /// <summary>
        /// 컬러 모드
        /// </summary>
        private ColorRole _colorMode = ColorRole.Color;
        public ColorRole ColorMode { get => _colorMode; set => SetProperty(ref _colorMode, value); }

    }

    /// <summary>
    /// 프레임(이미지)의 데이터 - 생성시각 바인딩용 모델
    /// </summary>
    public class FrameModel : BindableBase
    {
        /// <summary>
        /// 영상 데이터
        /// </summary>
        private Mat _image = new(1024, 1024, MatType.CV_8UC4, Scalar.White);
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }
        /// <summary>
        /// 프레임 생성 시각
        /// </summary>
        private DateTime _dateTime = DateTime.Now;
        public DateTime DateTime { get => _dateTime; set => SetProperty(ref _dateTime, value); }
    }

    /// <summary>
    /// 비디오 프레임(슬라이더) 재생/정보 상태 바인딩용 모델
    /// </summary>
    public class FrameInformationModel : BindableBase
    {
        /// <summary>
        /// 현재 슬라이더 번호
        /// </summary>
        private int _slider = 0;
        public int Slider { get => _slider; set => SetProperty(ref _slider, value); }

        /// <summary>
        /// 마지막 조작된 슬라이더 위치
        /// </summary>
        private int _lastestSlider = 0;
        public int LastestSlider { get => _lastestSlider; set => SetProperty(ref _lastestSlider, value); }

        /// <summary>
        /// 슬라이더 최대값
        /// </summary>
        private int _maxSlider = 100;
        public int MaxSlider { get => _maxSlider; set => SetProperty(ref _maxSlider, value); }

        /// <summary>
        /// 현재 재생 간격(프레임 간 이동 폭)
        /// </summary>
        private int _interval = 0;
        public int Interval { get => _interval; set => SetProperty(ref _interval, value); }

        /// <summary>
        /// 최대 재생 간격
        /// </summary>
        private int _maxInterval = 32;
        public int MaxInterval { get => _maxInterval; set => SetProperty(ref _maxInterval, value); }

        /// <summary>
        /// 전체 프레임 개수
        /// </summary>
        private int _count = 0;
        public int Count { get => _count; set => SetProperty(ref _count, value); }

        /// <summary>
        /// 비디오 캡처 프레임 개수
        /// </summary>
        private int _videoCaptureCount = 0;
        public int VideoCaptureCount { get => _videoCaptureCount; set => SetProperty(ref _videoCaptureCount, value); }

        /// <summary>
        /// 초당 프레임수(1000 / FPS)
        /// </summary>
        private int _fps = 120;
        public int FPS { get => _fps; set => SetProperty(ref _fps, value); }

        /// <summary>
        /// 비디오 재생 딜레이(1000 * VideoDelay)
        /// </summary>
        private int _videoDelay = 30;
        public int VideoDelay { get => _videoDelay; set => SetProperty(ref _videoDelay, value); }

        /// <summary>
        /// 가로 
        /// </summary>
        private double _width = 0;
        public double Width { get => _width; set => SetProperty(ref _width, value); }

        /// <summary>
        /// 세로
        /// </summary>
        private double _height = 0;
        public double Height { get => _height; set => SetProperty(ref _height, value); }

    }

    /// <summary>
    /// 이미지 정보를 나타내는 바인딩용 모델 
    /// </summary>
    public class ImageModel : BindableBase
    {
        /// <summary>
        /// 이미지의 소스 객체 (Bitmap, PNG, 기타 WPF 지원 소스)
        /// 값을 설정하면 <see cref="ImageBrush"/>도 자동 변경됨
        /// </summary>
        private ImageSource _imageSource = null;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                if (SetProperty(ref _imageSource, value))
                {
                    ImageBrush = new ImageBrush(ImageSource);
                }
            }
        }

        /// <summary>
        /// 현재 이미지에 적용되는 Brush (UI에서 이미지 Fill 등 용도)
        /// <see cref="ImageSource"/>가 변경될 때 자동 변경됨
        /// </summary>
        private Brush _imageBrush;
        public Brush ImageBrush { get => _imageBrush; set => SetProperty(ref _imageBrush, value); }

        /// <summary>
        /// 이미지에 적용되는 필터 정보 (Zoom, Brightness, Contrast 등)
        /// </summary>
        private FilterModel _filter = new();
        public FilterModel Filter { get => _filter; set => SetProperty(ref _filter, value); }

    }

    /// <summary>
    /// 실시간 미디어(영상/이미지) 처리 및 UI 출력을 위한 바인딩용 모델
    /// </summary>
    public class MediaModel : ImageModel
    {
        /// <summary>
        /// 영상을 출력할 대상 Canvas 객체 (UI 바인딩)
        /// </summary>
        public Canvas _screen;
        public Canvas Screen { get => _screen; set => SetProperty(ref _screen, value); }
        /// <summary>
        /// 현재 처리 중인 이미지(프레임) 데이터
        /// </summary>
        private Mat _image = new(1024, 1024, MatType.CV_8UC4, Scalar.White);
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }
        /// <summary>
        /// 재생/분석 중인 프레임의 목록
        /// </summary>
        private ObservableCollection<FrameModel> _frames = [];
        public ObservableCollection<FrameModel> Frames { get => _frames; set => SetProperty(ref _frames, value); }
        /// <summary>
        /// 미디어(영상/프레임)와 관련된 상태 정보 모델
        /// </summary>
        private FrameInformationModel _information = new();
        public FrameInformationModel Information { get => _information; set => SetProperty(ref _information, value); }

    }

    /// <summary>
    /// Xray 캘리브레이션(보정) 정보를 담는 바인딩용 모델
    /// </summary>
    public class CalibrationModel : BindableBase, ICalibration
    {
        /// <summary>
        /// 방사선이 켜진 상태의 센서값
        /// </summary>
        private Mat _gain;
        public Mat Gain { get => _gain; set => SetProperty(ref _gain, value); }
        /// <summary>
        /// 방사선이 꺼진 상태의 센서값
        /// </summary>
        private Mat _offset;
        public Mat Offset { get => _offset; set => SetProperty(ref _offset, value); }
        /// <summary>
        /// 방사선이 유무와 상관없이 축적된 데이터
        /// </summary>
        private Mat _origin;
        public Mat Origin { get => _origin; set => SetProperty(ref _origin, value); }
        /// <summary>
        /// 이미지 가로 방향의 상대적 비율(표준값 대비)
        /// </summary>
        private double _relativeWidthRatio = 1.25;
        public double RelativeWidthRatio { get => _relativeWidthRatio; set => SetProperty(ref _relativeWidthRatio, value); }
        /// <summary>
        /// 방사선이 꺼진 상태의 센서 최대 신호값
        /// </summary>
        private double _offsetRegion = 2800;
        public double OffsetRegion { get => _offsetRegion; set => SetProperty(ref _offsetRegion, value); }
        /// <summary>
        /// 방사선이 켜진 상태의 센서 최소 신호값
        /// </summary>
        private double _gainRegion = 15000;
        public double GainRegion { get => _gainRegion; set => SetProperty(ref _gainRegion, value); }
        /// <summary>
        /// Detector 사이의 빈 영역에서 발생하는 저신호 경계
        /// </summary>
        private double _boundaryArtifact = 3000;
        public double BoundaryArtifact { get => _boundaryArtifact; set => SetProperty(ref _boundaryArtifact, value); }
        /// <summary>
        /// 센서가 물체를 검출하기 위한 최소 신호 값(비율)
        /// </summary>
        private double _activationThresholdRatio = 0.9;
        public double ActivationThresholdRatio { get => _activationThresholdRatio; set => SetProperty(ref _activationThresholdRatio, value); }
        /// <summary>
        /// 이미지 최대 가로 길이(픽셀)
        /// </summary>
        private int _maxImageWidth = 1600;
        public int MaxImageWidth { get => _maxImageWidth; set => SetProperty(ref _maxImageWidth, value); }
        /// <summary>
        /// 센서에서 받아온 이미지 가로 길이(픽셀)
        /// </summary>
        private int _sensorImageWidth = 16;
        public int SensorImageWidth { get => _sensorImageWidth; set => SetProperty(ref _sensorImageWidth, value); }
    }

    /// <summary>
    /// 영상 분석의 물성 제어 파라미터를 관리하는 바인딩용 모델
    /// </summary>
    public class MaterialModel : BindableBase, IMaterial
    {
        /// <summary>
        /// 물성분석 이미지
        /// </summary>
        private Mat _image;
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }
        /// <summary>
        /// WPF 바인딩용 이미지 소스 (UI 렌더링용)
        /// </summary>
        private ImageSource _imageSource = null;
        public ImageSource ImageSource { get => _imageSource; set => SetProperty(ref _imageSource, value); }
        /// <summary>
        /// 물성 제어용 세부 파라미터 리스트
        /// </summary>
        private ObservableCollection<MaterialControlModel> _controls = [];
        public ObservableCollection<MaterialControlModel> Controls { get => _controls; set => SetProperty(ref _controls, value); }
        /// <summary>
        /// Blur
        /// </summary>
        private double _blur = 10.0;
        public double Blur { get => _blur; set => SetProperty(ref _blur, value); }
        /// <summary>
        /// 경계 비율
        /// (ex 1:1.05)
        /// </summary>
        private double _highLowRate = 1.05;
        public double HighLowRate { get => _highLowRate; set => SetProperty(ref _highLowRate, value); }
        /// <summary>
        /// 밀도(Density)
        /// </summary>
        private double _density = 1.5;
        public double Density { get => _density; set => SetProperty(ref _density, value); }
        /// <summary>
        /// 에지(Edge)
        /// </summary>
        private double _edgeBinary = 0.0;
        public double EdgeBinary { get => _edgeBinary; set => SetProperty(ref _edgeBinary, value); }
        /// <summary>
        /// 투명도(Transparency)
        /// </summary>
        private double _transparency = 1.8;
        public double Transparency { get => _transparency; set => SetProperty(ref _transparency, value); }

    }

    /// <summary>
    /// 물성 분석용 조작 컨트롤을 관리하는 바인딩용 모델
    /// </summary>
    public class MaterialControlModel : BindableBase, IMaterialControl
    {
        /// <summary>
        /// 액션 델리게이트를 약한 참조(WeakReference)로 저장  
        /// - 값 변경시 연동된 액션(콜백) 호출에 사용  
        /// </summary>
        public readonly WeakReference<Action> _action;

        /// <summary>
        /// 액션(값 변경시 수행할 함수) 주입
        /// </summary>
        public MaterialControlModel(Action action)
        {
            _action = new WeakReference<Action>(action);
        }
        /// <summary>
        /// 고유 번호
        /// </summary>
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        /// <summary>
        /// Y
        /// </summary>
        private double _y = 0.0;
        public double Y { get => _y; set { if (SetProperty(ref _y, value)) { Invoke(); } } }
        /// <summary>
        /// X 최소값
        /// </summary>
        private double _xMin = byte.MinValue;
        public double XMin { get => _xMin; set { if (SetProperty(ref _xMin, value)) { Invoke(); } } }
        /// <summary>
        /// X 최대값
        /// </summary>
        private double _xMax = byte.MaxValue;
        public double XMax { get => _xMax; set { if (SetProperty(ref _xMax, value)) { Invoke(); } } }
        /// <summary>
        /// 색상 정보(Scalar, OpenCV)  
        /// - (B,G,R,A) 순서로 컬러값 저장  
        /// - 변경시 WPF Color로 변환 및 콜백(Invoke) 호출
        /// </summary>
        private Scalar _scalar = new (byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        public Scalar Scalar
        {
            get => _scalar;
            set
            {
                if (SetProperty(ref _scalar, value))
                {
                    _color = Color.FromRgb((byte)value.Val2, (byte)value.Val1, (byte)value.Val0);
                    RaisePropertyChanged(nameof(Color));
                    RaisePropertyChanged(nameof(ColorBrush));

                    Invoke();
                }
            }
        }
        /// <summary>
        /// WPF 색상 정보  
        /// - UI 표시용(Color)  
        /// - 변경시 Scalar와 동기화 및 콜백(Invoke) 호출
        /// </summary>
        private Color _color;
        public Color Color 
        {
            get => _color;
            set
            {
                if (SetProperty(ref _color, value))
                {
                    _scalar = new Scalar(value.B, value.G, value.R, byte.MaxValue);
                    RaisePropertyChanged(nameof(Scalar));
                    RaisePropertyChanged(nameof(ColorBrush));

                    Invoke();
                }
            }
        }
        /// <summary>
        /// 색상 브러시  
        /// - 바인딩용 WPF SolidColorBrush
        /// </summary>
        public Brush ColorBrush => new SolidColorBrush(Color);
        /// <summary>
        /// 값 변경시 콜백 액션을 호출  
        /// - 바인딩/슬라이더/수식 등 값 변화에 실시간 반응
        /// </summary>
        private void Invoke()
        {
            if (_action.TryGetTarget(out var action))
            {
                action();
            }
        }
    }

    /// <summary>
    /// Zeffect 영상 데이터 및 제어 포인트 집합을 관리하는 바인딩용 모델
    /// </summary>
    public class ZeffectModel : BindableBase
    {
        /// <summary>
        /// 영상 데이터
        /// </summary>
        private Mat _image = new(1024, 1024, MatType.CV_8UC1, Scalar.White);
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }

        /// <summary>
        /// WPF UI 표시용 이미지 (ImageSource)
        /// - ImageSource 설정 시, ImageBrush 자동 동기화
        /// </summary>
        private ImageSource _imageSource = null;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                if (SetProperty(ref _imageSource, value))
                {
                    ImageBrush = new ImageBrush(ImageSource);
                }
            }
        }
        /// <summary>
        /// 이미지 표현용 WPF 브러시  
        /// - ImageSource가 변경될 때 동기화
        /// </summary>
        private Brush _imageBrush;
        public Brush ImageBrush { get => _imageBrush; set => SetProperty(ref _imageBrush, value); }
        /// <summary>
        /// 영상 프레임 시퀀스
        /// </summary>
        private ObservableCollection<FrameModel> _frames = [];
        public ObservableCollection<FrameModel> Frames { get => _frames; set => SetProperty(ref _frames, value); }
        /// <summary>
        /// 현재 선택된 Z-effect 제어
        /// </summary>
        private ZeffectControlModel _control = new();
        public ZeffectControlModel Control 
        { 
            get => _control;
            set
            {
                if(SetProperty(ref _control, value))
                {
                    foreach (var c in Controls)
                    {
                        c.Check = (c == value);
                    }
                }
            }
        }
        /// <summary>
        /// Z-effect 제어 포인트 목록  
        /// </summary>
        private ObservableCollection<ZeffectControlModel> _controls = [];
        public ObservableCollection<ZeffectControlModel> Controls { get => _controls; set => SetProperty(ref _controls, value); }


    }

    /// <summary>
    /// Z-effect 개별 제어 바인딩용 모델  
    /// </summary>
    public class ZeffectControlModel : BindableBase, IZeffectControl
    {
        /// <summary>
        /// 고유 번호
        /// </summary>
        private int _id ;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        /// <summary>
        /// 커스텀 적용 여부
        /// </summary>
        private bool _check = false;
        public bool Check { get => _check; set => SetProperty(ref _check, value); }
        /// <summary>
        /// 커스텀 설명(명칭)
        /// </summary>
        private string _content = string.Empty;
        public string Content { get => _content; set => SetProperty(ref _content, value); }
        /// <summary>
        /// 커스텀 적용 최소값
        /// </summary>
        private double _min = 0.0;
        public double Min { get => _min; set => SetProperty(ref _min, Math.Clamp(value, 0.0f, Max)); }
        /// <summary>
        /// 커스텀 적용 최대값
        /// </summary>
        private double _max = 1.0;
        public double Max { get => _max; set => SetProperty(ref _max, Math.Clamp(value, Min, 1.0f)); }
        /// <summary>
        /// 포인트별 색상 정보(Scalar, BGRA)  
        /// - 변경시 Color와 Brush 동기화
        /// </summary>
        private Scalar _scalar = new(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        public Scalar Scalar
        {
            get => _scalar;
            set
            {
                if (SetProperty(ref _scalar, value))
                {
                    _color = Color.FromArgb((byte)value.Val3, (byte)value.Val2, (byte)value.Val1, (byte)value.Val0);
                    RaisePropertyChanged(nameof(Color));
                    RaisePropertyChanged(nameof(ColorBrush));
                }
            }
        }
        /// <summary>
        /// WPF 색상 정보 (ARGB)
        /// - 변경시 Scalar와 Brush 동기화
        /// </summary>
        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (SetProperty(ref _color, value))
                {
                    _scalar = new Scalar(value.B, value.G, value.R, value.A);
                    RaisePropertyChanged(nameof(Scalar));
                    RaisePropertyChanged(nameof(ColorBrush));
                }
            }
        }
        /// <summary>
        /// 색상 표현용 Brush (UI 바인딩)
        /// </summary>
        public Brush ColorBrush => new SolidColorBrush(Color);
    }
}