using System;
using System.Threading.Tasks;
using MZ.Vision;
using MZ.Domain.Models;
using OpenCvSharp;
using Prism.Mvvm;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// Calibration 처리를 담당하는 프로세스
    /// 이미지 데이터 및 파라미터를 관리 및 연산 수행
    /// </summary>
    public class CalibrationProcesser : BindableBase
    {
        #region Params
        private CalibrationModel _model = new();
        public CalibrationModel Model { get => _model; set => SetProperty(ref _model, value); }

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

        /// <summary>
        /// 이미지 생성
        /// </summary>
        public void Create(int width, int height)
        {
            Model.Origin = VisionBase.Create(height, width, MatType.CV_16UC1, new Scalar(0));
        }

        /// <summary>
        /// 원본 이미지를 비율에 맞춰 리사이즈
        /// </summary>
        public Mat AdjustRatio(Mat origin)
        {
            int width = (int)(origin.Width / Model.RelativeWidthRatio);
            Model.SensorImageWidth = width;
            return VisionBase.Resize(origin, width, origin.Height);
        }

        /// <summary>
        /// 켄버스(UI) 크기가 변경 되었을 경우 이미지 크기 갱신
        /// </summary>
        public void UpdateOnResize(Mat line, int width)
        {
            if (width != Model.MaxImageWidth)
            {
                Model.Origin = VisionBase.Create(line.Height, (int)Model.MaxImageWidth, MatType.CV_16UC1, new Scalar(0));
            }
        }

        /// <summary>
        /// 켄버스(UI) 크기가 변경 되었을 경우 이미지 크기 갱신 (비동기)
        /// </summary>
        public async Task UpdateOnResizeAsync(Mat line, int width)
        {
            await Task.Run(() => {
                UpdateOnResize(line, width);
            });
        }

        /// <summary>
        /// Offset/Gain의 영역에 따라 값 갱신
        /// </summary>
        public bool UpdateOnEnergy(Mat line)
        {
            (_, double max) = VisionBase.MinMax(line);

            bool check = true;
            if (max < Model.OffsetRegion)
            {
                Model.Offset = line;
                check = false;
            }
            else if (max < Model.GainRegion)
            {
                Model.Gain = line;
                check = false;
            }
            return check;
        }

        /// <summary>
        /// 이미지 Shift 수행
        /// </summary>
        public void Shift(Mat line)
        {
            Model.Origin = VisionBase.ShiftCol(Model.Origin, line);
        }

        /// <summary>
        /// 이미지 Shift 수행 (비동기)
        /// </summary>
        public async Task ShiftAsync(Mat line)
        {
            await Task.Run(() =>
            {
                Shift(line);
            });
        }

        /// <summary>
        /// 데이터 정규화 수행(Calibration Algorithm)
        /// </summary>
        public double Normalize(double value, double offset, double gain, double rate)
        {
            double result = (value - offset) / Math.Max(1.0, (gain - offset)) * rate;
            return Math.Clamp(result, 0.0, 1.0);
        }

        /// <summary>
        /// 해당 이미지에 물체 유무 확인
        /// </summary>
        public bool IsObject(Mat high)
        {
            if (high == null || high.Empty())
            {
                return false;
            }

            using Mat converted = VisionBase.Convert16BitTo8Bit(high);

            var checkThresholdTask = IsObjectCheckThreshold(converted, Model.ActivationThresholdRatio);
            var checkTopPixelTask = IsObjectCheckTopPixel(converted, Model.ActivationThresholdRatio);

            return checkThresholdTask && checkTopPixelTask;
        }

        /// <summary>
        /// 해당 이미지에 물체 유무 확인 (비동기)
        /// </summary>
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

        /// <summary>
        /// Threshold를 기준으로 한 물건 유무 탐지
        /// </summary>
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

        /// <summary>
        /// 최상단 픽셀을 기준으로 한 물건 유무 탐지
        /// </summary>
        private bool IsObjectCheckTopPixel(Mat convert, double maxPercent)
        {
            using Mat oneLine = convert.RowRange(0, 1);
            (double checkMin, _) = VisionBase.MinMax(oneLine);

            return checkMin > byte.MaxValue * maxPercent;
        }

        /// <summary>
        /// Gain값이 해당 boundary 이상 유무 확인
        /// </summary>
        public bool CompareBoundaryArtifact(double gain)
        {
            return gain > Model.BoundaryArtifact;
        }

        /// <summary>
        /// Gain값 갱신
        /// </summary>
        public void UpdateGain(Mat line)
        {
            Model.Gain = line;
        }

    }
}