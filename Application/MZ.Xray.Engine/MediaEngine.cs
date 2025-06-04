using MZ.Domain.Enums;
using MZ.Domain.Models;
using MZ.Vision;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Prism.Mvvm;

namespace MZ.Xray.Engine
{
    public class MediaEngine : BindableBase
    {

        #region Vision Algorithm (OpenCV)
        private readonly VisionBase _visionBase = new();
        private readonly VisionLUT _visionLUT = new();
        #endregion

        #region Params
        private MediaModel _model = new();
        public MediaModel Model { get => _model; set => SetProperty(ref _model, value); }
        #endregion

        public void Create(int width, int height)
        {
            Model.Image = _visionBase.Create(height, width, MatType.CV_8UC4);
            Model.ImageSource = Model.Image.ToBitmapSource();
        }

        public void UpdateOnResize(Mat line, int width)
        {
            Model.Image = (GetWidth() != width)
                ? _visionBase.Create((line.Height / 2), width, MatType.CV_8UC4, new Scalar(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue))
                : Model.Image;
        }

        public void Shift(Mat color)
        {
            Model.Image = _visionBase.ShiftCol(Model.Image, color);
        }

        public void IncreaseCount()
        {
            Model.Information.Count++;
        }

        public void ClearCount()
        {
            Model.Information.Count = 0;
        }


        public void SetZoom(float zoom)
        {
            Model.Filter.Zoom = zoom;
        }

        public void SetBrightness(float level)
        {
            Model.Filter.Brightness = level;
        }

        public void SetContrast(float level)
        {
            Model.Filter.Contrast = level;
        }

        public void SetSharpness(float level)
        {
            Model.Filter.Sharpness = level;
        }

        public void SetColorMode(ColorRole mode)
        {
            Model.Filter.ColorMode = mode;
        }

        public Size GetImageSize()
        {
            return Model.Image.Size();
        }

        public int GetWidth()
        {
            return Model.Image.Width;
        }

        public int GetHeight()
        {
            return Model.Image.Height;
        }
    }
}
