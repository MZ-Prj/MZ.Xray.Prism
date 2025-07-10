using MZ.Domain.Models;
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
using MZ.Domain.Enums;

namespace MZ.Xray.Engine
{
    public class MediaProcesser : BindableBase
    {
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

        public ImageSource ImageSource
        {
            get => Model.ImageSource;
            set => Model.ImageSource = value;
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

        public void Create(int width, int height)
        {
            Image = VisionBase.Create(height, width, MatType.CV_8UC4);
            ImageSource = Image.ToBitmapSource();

            Information.Width = width;
            Information.Height = height;

            Information.Size = new(width, height);
        }

        public void UpdateOnResize(Mat line, int width)
        {
            if (Image.Width != width)
            {
                Image = VisionBase.Create((line.Height / 2), width, MatType.CV_8UC4, new Scalar(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0));
            }
        }

        public async Task UpdateOnResizeAsync(Mat line, int width)
        {
            await Task.Run(() =>
            {
                UpdateOnResize(line, width);
            });
        }

        public void ChangedSlider()
        {
            if (IsShowFrame(Information.Slider))
            {
                ImageSource = CanFreezeImageSource(Frames[Information.Slider-1].Image.ToBitmapSource());
            }
        }

        public void Update()
        {
            ImageSource = CanFreezeImageSource(Image.ToBitmapSource());

            if (Information.Interval % Information.MaxInterval == 0)
            {
                Frames.Add(new() { Image = Image, DateTime = DateTime.Now });

                if (Information.Slider >= Information.MaxSlider)
                {
                    Frames.RemoveAt(0);
                    Information.Slider = Information.MaxSlider;
                    Information.LastestSlider = Information.MaxSlider; 
                }
                Information.Slider++;
                Information.LastestSlider++;
                Information.Interval = 0;
            }
            Information.Interval++;
        }

        public void Shift(Mat color)
        {
            Image = VisionBase.ShiftCol(Image, color);
        }

        public async Task ShiftAsync(Mat color)
        {
            await Task.Run(() =>
            {
                Shift(color);
            });
        }

        public void PrevNextSlider(int rule)
        {
            int slider = Information.Slider + rule;
            if (IsShowFrame(slider))
            {
                ImageSource = CanFreezeImageSource(Frames[slider-1].Image.ToBitmapSource());
                Information.Slider = slider;
            }
        }
        public void IncreaseCount()
        {
            Information.Count++;
        }

        public void ClearCount()
        {
            Information.Count = 0;
        }

        public void LastestSlider()
        {
            Information.Slider = Information.LastestSlider;
        }

        public void ChangedFilterColor(ColorRole color)
        {
            Filter.ColorMode = color;
        }

        public void ChangedFilterBrightness(float bright)
        {
            Filter.Brightness += bright;
        }

        public void ChangedFilterContrast(float contrast)
        {
            Filter.Contrast += contrast;
        }

        public void ChangedFilterZoom(float zoom)
        {
            Filter.Zoom += zoom;
        }

        public void ClearFilter()
        {
            Filter = new ();
        }

        private bool IsShowFrame(int slider)
        {
            return slider <= Frames.Count && slider > 0 && slider <= Information.MaxSlider;
        }

        private BitmapSource CanFreezeImageSource(BitmapSource bitmap)
        {
            if (bitmap.CanFreeze)
            {
                bitmap.Freeze();
            }
            return bitmap;
        }

    }
}
