using MZ.Domain.Models;
using MZ.Domain.Enums;
using MZ.Vision;
using Prism.Commands;
using Prism.Mvvm;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// 영상 처리 및 제어 프로세스
    /// </summary>
    public class MediaProcesser : BindableBase
    {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        #region Params
        private MediaModel _model = new();
        public MediaModel Model { get => _model; set => SetProperty(ref _model, value); }
        #endregion

        #region Command

        private DelegateCommand _changedSliderCommand;
        public ICommand ChangedSliderCommand => _changedSliderCommand ??= new DelegateCommand(ChangedSlider);

        #endregion

        #region Wrapper

        public Mat Image
        {
            get => Model.Image;
            set => Model.Image = value;
        }

        public Canvas Screen
        {
            get => Model.Screen;
            set => Model.Screen = value;
        }

        public ImageSource ImageSource
        {
            get => Model.ImageSource;
            set => Model.ImageSource = value;
        }

        public ImageSource Histogram
        {
            get => Model.Histogram;
            set => Model.Histogram = value;
        }
        public FilterModel Filter
        {
            get => Model.Filter;
            set => Model.Filter = value;
        }

        public FrameInformationModel Information
        {
            get => Model.Information;
            set => Model.Information = value;
        }

        public ObservableCollection<FrameModel> Frames
        {
            get => Model.Frames;
            set => Model.Frames = value;
        }

        #endregion

        /// <summary>
        /// Canvas(UI) 기준 가로 세로값 정의 및 파라메터 생성
        /// </summary>
        public void Create(int width, int height)
        {
            Image = VisionBase.Create(height, width, MatType.CV_8UC4);
            ImageSource = Image.ToBitmapSource();

            Information.Width = width;
            Information.Height = height;

            Filter.Size = new(width, height);
        }

        /// <summary>
        /// 켄버스(UI) 크기가 변경 되었을 경우 이미지 크기 갱신
        /// </summary>
        public void UpdateOnResize(Mat line, int width)
        {
            if (Image.Width != width)
            {
                Image = VisionBase.Create((line.Height / 2), width, MatType.CV_8UC4, new Scalar(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0));
            }
        }

        /// <summary>
        /// 켄버스(UI) 크기가 변경 되었을 경우 이미지 크기 갱신 (비동기)
        /// </summary>
        public async Task UpdateOnResizeAsync(Mat line, int width)
        {
            await Task.Run(() =>
            {
                UpdateOnResize(line, width);
            });
        }

        /// <summary>
        /// 슬라이더 정보 변경시 ImageSource 갱신
        /// </summary>
        public void ChangedSlider()
        {
            if (IsShowFrame(Information.Slider))
            {
                ImageSource = VisionBase.CanFreezeImageSource(Frames[Information.Slider-1].Image.ToBitmapSource());
            }
        }

        /// <summary>
        /// Video 형식으로 저장
        /// </summary>
        public void SaveVideo()
        {
            string path = XrayDataSaveManager.GetPath();
            string time = XrayDataSaveManager.GetCurrentTime();

            Task.Run(async () =>
            {
                await XrayDataSaveManager.VideoAsync([.. Frames], path, $"{time}.avi");
            });
        }

        /// <summary>
        /// 이미지 Shift 수행
        /// </summary>
        public void Shift(Mat color)
        {
            Image = VisionBase.ShiftCol(Image, color);
        }

        /// <summary>
        /// 이미지 Shift 수행 (비동기)
        /// </summary>
        public async Task ShiftAsync(Mat color)
        {
            await Task.Run(() =>
            {
                Shift(color);
            });
        }

        /// <summary>
        /// 이전 이후 Slider 값 계산
        /// </summary>
        public int PrevNextSlider(int rule)
        {
            int slider = Information.Slider + rule;
            if (IsShowFrame(slider))
            {
                Information.Slider = slider;
            }
            return slider;
        }

        /// <summary>
        /// ImageSource 갱신
        /// </summary>
        public void ChangeFrame(int index)
        {
            ImageSource = VisionBase.CanFreezeImageSource(Frames[index-1].Image.ToBitmapSource());
        }

        /// <summary>
        /// 실시간 미디어(영상) Count 증가
        /// </summary>
        public void IncreaseCount()
        {
            Information.Count++;
        }

        /// <summary>
        /// 실시간 미디어(영상) Count 초기화
        /// </summary>
        public void ClearCount()
        {
            Information.Count = 0;
        }

        /// <summary>
        /// 최근 Sldiert 값 갱신(중지 후 시작 할경우 가장 최근 Sldier를 불러옴)
        /// </summary>
        public void LastestSlider()
        {
            Information.Slider = Information.LastestSlider;
        }

        /// <summary>
        /// Filter 색상 변경
        /// </summary>
        public void ChangedFilterColor(ColorRole color)
        {
            Filter.ColorMode = color;
        }

        /// <summary>
        /// 밝기 조절
        /// </summary>
        public void ChangedFilterBrightness(float bright)
        {
            Filter.Brightness += bright;
        }
        /// <summary>
        /// 명암비 조절
        /// </summary>
        public void ChangedFilterContrast(float contrast)
        {
            Filter.Contrast += contrast;
        }
        /// <summary>
        /// 확대/축소 조절(Zoom)
        /// </summary>
        public void ChangedFilterZoom(float zoom)
        {
            Filter.Zoom += zoom;
        }

        /// <summary>
        /// 필터 초기화
        /// </summary>
        public void ClearFilter()
        {
            Filter = new ();
        }

        /// <summary>
        /// UI에 그려진 Element값을 이미지(mat)로 변환
        /// </summary>
        public Mat ChangedScreenToMat()
        {
            Mat mat = null;
            _dispatcher.Invoke(() =>
            {
                var parent = Screen.Parent as FrameworkElement;
                parent.UpdateLayout();

                double width = parent.ActualWidth;
                double height = parent.ActualHeight;

                RenderTargetBitmap renderBitmap = new((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);
                renderBitmap.Render(parent);

                var converted = new FormatConvertedBitmap(renderBitmap, PixelFormats.Bgra32, null, 0);
                mat = BitmapSourceConverter.ToMat(converted);
            });
            
            return mat;

        }

        /// <summary>
        /// 현제 프레임이 보여지는 slider인지 검증
        /// </summary>
        public bool IsShowFrame(int slider)
        {
            return slider <= Frames.Count && slider > 0 && slider <= Information.MaxSlider;
        }

        /// <summary>
        /// Mat -> ImageSource로 변환
        /// </summary>
        public void FreezeImageSource()
        {
            ImageSource = VisionBase.CanFreezeImageSource(Image.ToBitmapSource());
            Histogram = VisionBase.CanFreezeImageSource(VisionBase.PlotFilledHistogram(Image).ToBitmapSource());
        }
        /// <summary>
        /// Mat -> ImageSource로 변환 (비동기)
        /// </summary>
        public async Task FreezeImageSourceAsync()
        {
            await Task.Run(FreezeImageSource);
        }

        /// <summary>
        /// 영상(Video)이 보여질때 화면 갱신 여부
        /// </summary>
        public bool IsFrameUpdateRequired()
        {
            return Information.Interval % Information.MaxInterval == 0;
        }
        /// <summary>
        /// 현제 프레임 추가
        /// </summary>
        public void AddFrame()
        {
            Frames.Add(new() { Image = Image, DateTime = DateTime.Now });
        }

        /// <summary>
        /// Slider 초기화
        /// </summary>
        public void ResetSlider()
        {
            Information.Slider = Information.MaxSlider;
            Information.LastestSlider = Information.MaxSlider;
        }

        /// <summary>
        /// Slider 증가 및 Interval 제어
        /// </summary>
        public void IncrementSlider()
        {
            Information.Slider++;
            Information.LastestSlider++;
            Information.Interval = 0;
        }

        /// <summary>
        /// Interval 증가
        /// </summary>
        public void IncreaseInterval()
        {
            Information.Interval++;
        }

        /// <summary>
        /// 가장 첫번째 Frame 제거
        /// </summary>
        public void RemoveFrame()
        {
            Frames.RemoveAt(0);
        }

    }
}
