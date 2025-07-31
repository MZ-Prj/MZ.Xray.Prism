using System;
using MZ.Vision;
using MZ.Domain.Models;
using OpenCvSharp;
using Prism.Mvvm;
using static MZ.Vision.VisionEnums;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp.WpfExtensions;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// 물성에 대한 정보를 수치로 표기
    /// </summary>
    public class ZeffectProcesser : BindableBase
    {
        #region Params
        private ZeffectModel _model = new();
        public ZeffectModel Model { get => _model; set => SetProperty(ref _model, value); }

        public ObservableCollection<ZeffectControlModel> Controls
        {
            get => Model.Controls;
            set => Model.Controls = value;
        }

        public Brush ImageBrush
        {
            get => Model.ImageBrush;
            set => Model.ImageBrush = value;
        }

        public ImageSource ImageSource
        {
            get => Model.ImageSource;
            set => Model.ImageSource = value;
        }

        public Mat Image
        {
            get => Model.Image;
            set => Model.Image = value;
        }
        #endregion

        public byte Calculation(double high, double low)
        {
            if (high <= 0 || low <= 0)
            {
                return 0;
            }

            double norm = ushort.MaxValue;
            double ratioHigh = high / norm;
            double ratioLow = low / norm;

            double epsilon = 1e-10;
            if (ratioHigh >= 1.0)
            {
                ratioHigh = 1.0 - epsilon;
            }
            if (ratioLow >= 1.0)
            {
                ratioLow = 1.0 - epsilon;
            }

            double avg = (ratioHigh + ratioLow) / 2.0;
            double diff = (ratioHigh - ratioLow);

            if (Math.Abs(avg) < epsilon)
            {
                return 0;
            }

            double zeff = Math.Pow(VisionLUT.Run(FunctionNameEnumTypes.Pow, diff / avg, [1.0]), 1.0);

            if (double.IsNaN(zeff) || double.IsInfinity(zeff))
            {
                return 0;
            }

            return (byte)(zeff * byte.MaxValue);
        }

        public void UpdateOnResize(Mat line, int width, int maxImageWidth)
        {
            if (width != maxImageWidth)
            {
                Image = VisionBase.Create((line.Height / 2), maxImageWidth, MatType.CV_8UC1, new Scalar(0));
            }
        }

        public async Task UpdateOnResizeAsync(Mat line, int width, int maxImageWidth)
        {
            await Task.Run(() =>
            {
                UpdateOnResize(line, width, maxImageWidth);
            });
        }

        public void Shift(Mat zeff)
        {
            Image = VisionBase.ShiftCol(Model.Image, zeff);
        }

        public async Task ShiftAsync(Mat zeff)
        {
            await Task.Run(() =>
            {
                Shift(zeff);
            });
        }

        public void FreezeImageSource()
        {
            ImageSource = CanFreezeImageSource(Image.ToBitmapSource());
        }

        public BitmapSource CanFreezeImageSource(BitmapSource bitmap)
        {
            if (bitmap.CanFreeze)
            {
                bitmap.Freeze();
            }
            return bitmap;
        }
    }
}
