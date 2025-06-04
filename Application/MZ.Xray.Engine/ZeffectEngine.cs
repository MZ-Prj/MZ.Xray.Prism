using MZ.Domain.Models;
using MZ.Vision;
using OpenCvSharp;
using Prism.Mvvm;
using System;
using static MZ.Vision.VisionEnums;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// 물성에 대한 정보를 수치로 표기
    /// </summary>
    public class ZeffectEngine : BindableBase
    {
        #region Vision Algorithm (OpenCV)
        private readonly VisionBase _visionBase = new();
        private readonly VisionLUT _visionLUT = new();
        #endregion

        #region Params
        private ZeffectModel _model = new();
        public ZeffectModel Model { get => _model; set => SetProperty(ref _model, value); }
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

            double zeff = Math.Pow(_visionLUT.Run(FunctionNameEnumTypes.Pow, diff / avg, [1.0]), 1.0);

            if (double.IsNaN(zeff) || double.IsInfinity(zeff))
            {
                return 0;
            }

            return (byte)(zeff * byte.MaxValue);
        }

        public void UpdateOnResize(Mat line, int width, int maxImageWidth)
        {
            Model.Image = (width != maxImageWidth)
                ? _visionBase.Create((line.Height / 2), maxImageWidth, MatType.CV_8UC4, new Scalar(0)) : Model.Image;
        }

        public void Shift(Mat zeff)
        {
            Model.Image = _visionBase.ShiftCol(Model.Image, zeff);
        }

    }
}
