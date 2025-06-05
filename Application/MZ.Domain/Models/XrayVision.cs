using System;
using MZ.Domain.Interfaces;
using MZ.Domain.Enums;
using OpenCvSharp;
using Prism.Mvvm;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace MZ.Domain.Models
{

    public class FilterModel : BindableBase, IFilter
    {
        private float _zoom = 1.0f;
        public float Zoom { get => _zoom; set => SetProperty(ref _zoom, value); }

        private float _sharpness = 0.0f;
        public float Sharpness { get => _sharpness; set => SetProperty(ref _sharpness, value); }

        private float _brightness = 0.0f;
        public float Brightness { get => _brightness; set => SetProperty(ref _brightness, value); }

        private float _contrast = 2.0f;
        public float Contrast { get => _contrast; set => SetProperty(ref _contrast, value); }

        private ColorRole _colorMode = ColorRole.Color;
        public ColorRole ColorMode { get => _colorMode; set => SetProperty(ref _colorMode, value); }

    }

    public class FrameModel : BindableBase
    {
        private Mat _image = new(1024, 1024, MatType.CV_8UC4, Scalar.White);
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }

        private DateTime _dateTime = DateTime.Now;
        public DateTime DateTime { get => _dateTime; set => SetProperty(ref _dateTime, value); }
    }


    public class FrameInformationModel : BindableBase
    {

        private int _slider = 0;
        public int Slider { get => _slider; set => SetProperty(ref _slider, value); }

        private int _count = 0;
        public int Count { get => _count; set => SetProperty(ref _count, value); }

        private int _captureCount = 0;
        public int CaptureCount { get => _captureCount; set => SetProperty(ref _captureCount, value); }

        private int _captureCapacity = 32;
        public int CaptureCapacity { get => _captureCapacity; set => SetProperty(ref _captureCapacity, value); }

        private int _fps = 60;
        public int FPS { get => _fps; set => SetProperty(ref _fps, value); }

        private int _maxStackCount = 100;
        public int MaxStackCount { get => _maxStackCount; set => SetProperty(ref _maxStackCount, value); }

        private int _stackCount = 0;
        public int StackCount { get => _stackCount; set => SetProperty(ref _stackCount, value); }

        private bool _isPlaying = false;
        public bool IsPlaying { get => _isPlaying; set => SetProperty(ref _isPlaying, value); }

    }

    public class MediaModel : BindableBase
    {
        private Mat _image = new(1024, 1024, MatType.CV_8UC4, Scalar.White);
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }

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

        private Brush _imageBrush;
        public Brush ImageBrush { get => _imageBrush; set => SetProperty(ref _imageBrush, value); }

        private FilterModel _filter = new();
        public FilterModel Filter { get => _filter; set => SetProperty(ref _filter, value); }

        private ObservableCollection<FrameModel> _frames = [];
        public ObservableCollection<FrameModel> Frames { get => _frames; set => SetProperty(ref _frames, value); }

        private FrameInformationModel _information = new();
        public FrameInformationModel Information { get => _information; set => SetProperty(ref _information, value); }

    }

    public class CalibrationModel : BindableBase, ICalibration
    {
        private Mat _gain;
        public Mat Gain { get => _gain; set => SetProperty(ref _gain, value); }

        private Mat _offset;
        public Mat Offset { get => _offset; set => SetProperty(ref _offset, value); }

        private Mat _origin;
        public Mat Origin { get => _origin; set => SetProperty(ref _origin, value); }

        private double _relativeWidthRatio = 1.25;
        public double RelativeWidthRatio { get => _relativeWidthRatio; set => SetProperty(ref _relativeWidthRatio, value); }

        private double _offsetRegion = 15000;
        public double OffsetRegion { get => _offsetRegion; set => SetProperty(ref _offsetRegion, value); }

        private double _gainRegion = 2600;
        public double GainRegion { get => _gainRegion; set => SetProperty(ref _gainRegion, value); }

        private double _boundaryArtifact = 5000;
        public double BoundaryArtifact { get => _boundaryArtifact; set => SetProperty(ref _boundaryArtifact, value); }
        
        private double _activationThresholdRatio = 0.9;
        public double ActivationThresholdRatio { get => _activationThresholdRatio; set => SetProperty(ref _activationThresholdRatio, value); }

        private int _maxImageWidth = 1600;
        public int MaxImageWidth { get => _maxImageWidth; set => SetProperty(ref _maxImageWidth, value); }

        private int _sensorImageWidth = 16;
        public int SensorImageWidth { get => _sensorImageWidth; set => SetProperty(ref _sensorImageWidth, value); }
    }

    public class MaterialModel : BindableBase, IMaterial
    {
        private Mat _image;
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }

        private ImageSource _imageSource;
        public ImageSource ImageSource { get => _imageSource; set => SetProperty(ref _imageSource, value); }

        private ObservableCollection<MaterialControlModel> _controls = [];
        public ObservableCollection<MaterialControlModel> Controls { get => _controls; set => SetProperty(ref _controls, value); }

        private double _blur = 10.0;
        public double Blur { get => _blur; set => SetProperty(ref _blur, value); }

        private double _highLowRate = 1.05;
        public double HighLowRate { get => _highLowRate; set => SetProperty(ref _highLowRate, value); }

        private double _density = 1.5;
        public double Density { get => _density; set => SetProperty(ref _density, value); }

        private double _edgeBinary = 0.0;
        public double EdgeBinary { get => _edgeBinary; set => SetProperty(ref _edgeBinary, value); }

        private double _transparency = 1.8;
        public double Transparency { get => _transparency; set => SetProperty(ref _transparency, value); }

    }

    public class MaterialControlModel : BindableBase, IMaterialControl
    {
        private double _y = 0.0;
        public double Y { get => _y; set => SetProperty(ref _y, value); }

        private double _xMin = byte.MinValue;
        public double XMin { get => _xMin; set => SetProperty(ref _xMin, value); }

        private double _xMax = byte.MaxValue;
        public double XMax { get => _xMax; set => SetProperty(ref _xMax, value); }

        private Scalar _scalar = new (byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        public Scalar Scalar
        {
            get => _scalar;
            set
            {
                if (SetProperty(ref _scalar, value))
                {
                    RaisePropertyChanged(nameof(ColorBrush));
                }
            }
        }

        public Brush ColorBrush
        {
            get
            {
                byte blue = (byte)Scalar.Val0;
                byte green = (byte)Scalar.Val1;
                byte red = (byte)Scalar.Val2;
                return new SolidColorBrush(Color.FromRgb(red, green, blue));
            }
        }
    }

    public class ZeffectModel : BindableBase
    {
        private Mat _image = new(1024, 1024, MatType.CV_8UC1, Scalar.White);
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }

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

        private Brush _imageBrush;
        public Brush ImageBrush { get => _imageBrush; set => SetProperty(ref _imageBrush, value); }

        private ZeffectControlModel _control;
        public ZeffectControlModel Control { get => _control; set => SetProperty(ref _control, value); }

        private ObservableCollection<ZeffectControlModel> _controls = [];
        public ObservableCollection<ZeffectControlModel> Controls { get => _controls; set => SetProperty(ref _controls, value); }
    }

    public class ZeffectControlModel : BindableBase, IZeffectControl
    {
        private bool _check = false;
        public bool Check { get => _check; set => SetProperty(ref _check, value); }

        private string _content = string.Empty;
        public string Content { get => _content; set => SetProperty(ref _content, value); }

        private double _min = 0.0;
        public double Min { get => _min; set => SetProperty(ref _min, value); }

        private double _max = 1.0;
        public double Max { get => _max; set => SetProperty(ref _max, value); }

    }
}