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

        public double RelativeWidthRatio { get; set; }
        public double OffsetRegion { get; set; }
        public double GainRegion { get; set; }
        public double BoundaryArtifact { get; set; }
        public double ActivationThresholdRatio { get; set; }
        public int MaxImageWidth { get; set; }
        public int SensorImageWidth { get; set; }
    }

    public class MaterialModel : BindableBase, IMaterial
    {
        private Mat _image;
        public Mat Image { get => _image; set => SetProperty(ref _image, value); }

        private ImageSource _imageSource;
        public ImageSource ImageSource { get => _imageSource; set => SetProperty(ref _imageSource, value); }

        private ObservableCollection<MaterialControlModel> _controls = [];
        public ObservableCollection<MaterialControlModel> Controls { get => _controls; set => SetProperty(ref _controls, value); }
        
        public double Blur { get; set; }
        public double HighLowRate { get; set; }
        public double Density { get; set; }
        public double EdgeBinary { get; set; }
        public double Transparency { get; set; }
    }

    public class MaterialControlModel : BindableBase, IMaterialControl
    {
        public double Y { get; set; }
        public double XMin { get; set; }
        public double XMax { get; set; }

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
        public bool Check { get; set; }
        public string Content { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }
}