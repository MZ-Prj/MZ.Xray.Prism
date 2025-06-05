using System;
using System.Threading.Tasks;
using MZ.Vision;
using MZ.Domain.Models;
using OpenCvSharp;
using Prism.Mvvm;

namespace MZ.Xray.Engine
{
    public class CalibrationProcesser : BindableBase
    {
        #region Params
        private CalibrationModel _model = new();
        public CalibrationModel Model { get => _model; set => SetProperty(ref _model, value); }
        #endregion

        #region Wrapper

        public Mat Origin
        {
            get => Model.Origin;
            set => Model.Origin = value;
        }

        public Mat Gain
        {
            get => Model.Gain;
            set => Model.Gain = value;
        }

        public Mat Offset
        {
            get => Model.Offset;
            set => Model.Offset = value;
        }

        public int MaxImageWidth
        {
            get => Model.MaxImageWidth;
            set => Model.MaxImageWidth = value;
        }

        public int SensorImageWidth
        {
            get => Model.SensorImageWidth;
            set => Model.SensorImageWidth = value;
        }

        #endregion

        public void Create(int width, int height)
        {
            Model.Origin = VisionBase.Create(height, width, MatType.CV_16UC1, new Scalar(0));
        }

        public Mat AdjustRatio(Mat origin)
        {
            int width = (int)(origin.Width / Model.RelativeWidthRatio);
            Model.SensorImageWidth = width;
            return VisionBase.Resize(origin, width, origin.Height);
        }

        public void UpdateOnResize(Mat line, int width)
        {
            Model.Gain ??= line;
            Model.Offset ??= line;

            Model.Origin = (width != Model.MaxImageWidth)
                ? VisionBase.Create(line.Height, (int)Model.MaxImageWidth, MatType.CV_16UC1, new Scalar(0))
                : Model.Origin;
        }

        public bool UpdateOnEnergy(Mat line)
        {
            (_, double max) = VisionBase.MinMax(line);

            if (max < Model.OffsetRegion)
            {
                Model.Offset = line;
            }
            else if (max < Model.GainRegion)
            {
                Model.Gain = line;
            }
            else
            {
                return false;
            }
            return true;
        }

        public void Shift(Mat line)
        {
            Model.Origin = VisionBase.ShiftCol(Model.Origin, line);
        }

        public double Normalize(ushort value, double offset, double gain, double rate)
        {
            double result = (value - offset) / Math.Max(1.0, (gain - offset)) * rate;
            return Math.Clamp(result, 0.0, 1.0);
        }

        public async Task<bool> IsObjectAsync(Mat high)
        {
            if (high == null || high.Empty())
            {
                return false;
            }

            using Mat converted = VisionBase.Convert16BitTo8Bit(high);

            var checkThresholdTask = Task.Run(() => IsObjectCheckThreshold(converted, Model.ActivationThresholdRatio));
            var checkTopPixelTask = Task.Run(() => IsObjectCheckTopPixel(converted, Model.ActivationThresholdRatio));

            var results = await Task.WhenAll(checkThresholdTask, checkTopPixelTask);

            return results[0] && results[1];
        }

        private bool IsObjectCheckThreshold(Mat convert, double maxPercent)
        {
            int size = (int)(convert.Width * 0.8);
            if (size % 2 == 0)
            {
                size++;
            }
            int threshold = (int)(byte.MaxValue * maxPercent);

            using Mat gaussianBlur = VisionBase.GaussianBlur(convert, size);
            using Mat thresholded = VisionBase.Threshold(gaussianBlur, threshold, ThresholdTypes.BinaryInv);

            (double checkMin, double checkMax) = VisionBase.MinMax(thresholded);
            return checkMax == byte.MaxValue;
        }

        private bool IsObjectCheckTopPixel(Mat convert, double maxPercent)
        {
            using Mat oneLine = convert.RowRange(0, 1);
            (double checkMin, _) = VisionBase.MinMax(oneLine);

            return checkMin > byte.MaxValue * maxPercent;
        }

        public bool CompareBoundaryArtifact(double gain)
        {
            return gain > Model.BoundaryArtifact;
        }

    }
}