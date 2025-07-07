using MZ.Domain.Models;
using MZ.Vision;
using Prism.Mvvm;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Threading.Tasks;

namespace MZ.Xray.Engine
{
    public class MediaProcesser : BindableBase
    {
        #region Params
        private MediaModel _model = new();
        public MediaModel Model { get => _model; set => SetProperty(ref _model, value); }
        #endregion

        #region Wrapper
        public int Slider
        {
            get => _model.Information.Slider;
            set
            {
                int slider = (value < 0 ? 0 : value);
                if (_model.Information.Slider != slider)
                {
                    _model.Information.Slider = slider;
                    if (_model.Frames.Count > 0 && _model.Information.Slider - 1 >= 0 && _model.Information.Slider - 1 < _model.Frames.Count)
                    {
                        var bitmap = _model.Frames[_model.Information.Slider - 1].Image.ToBitmapSource();
                        _model.ImageSource = VisionBase.CanFreezeImageSource(bitmap);
                    }
                    else
                    {
                        _model.ImageSource = null;
                    }
                }
            }
        }
        public Mat Image
        {
            get => Model.Image;
            set => Model.Image = value;
        }

        public FrameInformationModel Information
        {
            get => Model.Information;
            set => Model.Information = value;
        }
        #endregion

        public void Create(int width, int height)
        {
            Model.Image = VisionBase.Create(height, width, MatType.CV_8UC4);
            Model.ImageSource = Model.Image.ToBitmapSource();

            Model.Information.Width = width;
            Model.Information.Height = height;
        }

        public void UpdateOnResize(Mat line, int width)
        {
            Model.Image = (Model.Image.Width != width)
                ? VisionBase.Create((line.Height / 2), width, MatType.CV_8UC4, new Scalar(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue))
                : Model.Image;
        }

        public async Task UpdateOnResizeAsync(Mat line, int width)
        {
            await Task.Run(() =>
            {
                UpdateOnResize(line, width);
            });
        }

        public void Shift(Mat color)
        {
            Model.Image = VisionBase.ShiftCol(Model.Image, color);
        }

        public async Task ShiftAsync(Mat color)
        {
            await Task.Run(() =>
            {
                Shift(color);
            });
        }

        public void IncreaseCount()
        {
            Model.Information.Count++;
        }

        public void ClearCount()
        {
            Model.Information.Count = 0;
        }
    }
}
