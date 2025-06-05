using MZ.Domain.Models;
using MZ.Vision;
using OpenCvSharp;
using System;
using Prism.Mvvm;
using static MZ.Vision.VisionEnums;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// 물성에 대한 정보를 이미지로 표기
    /// </summary>
    public class MaterialProcesser : BindableBase
    {

        #region Params
        private MaterialModel _model = new();
        public MaterialModel Model { get => _model; set => SetProperty(ref _model, value); }
        #endregion

        #region Wrapper
        public double HighLowRate
        {
            get => Model.HighLowRate;
            set => Model.HighLowRate = value;
        }

        #endregion

        public Vec4b Calculation(double high, double low)
        {
            // 평균
            double avg = (high + low) * 0.5;

            // 차
            double diff = Math.Abs(high - low);

            // Material Image에서 색상 추출
            Vec4b color = Model.Image.At<Vec4b>((int)(diff * byte.MaxValue), (int)(avg * byte.MaxValue));

            // 연산
            double highLut = Math.Clamp(VisionLUT.Run(FunctionNameEnumTypes.Atanh, high, [1, 0, 1, 2]), 0.0, 1.0);
            double lowLut = Math.Clamp(VisionLUT.Run(FunctionNameEnumTypes.Atanh, low, [1, 0, 1, 2]), 0.0, 1.0);
            double avgLut = (highLut + lowLut) * 0.5;

            // 밀도(%)
            double density = Math.Clamp(avgLut * Model.Density, 0.0, 1.0);

            // 투영도(%)
            double transparency = Math.Clamp(VisionLUT.Run(FunctionNameEnumTypes.Log, ((1 - avg)), [Model.Transparency]), 0.0, 1.0);

            // 색상(byte) * 밀도(%)
            byte r = (byte)(color.Item2 * density);
            byte g = (byte)(color.Item1 * density);
            byte b = (byte)(color.Item0 * density);

            // 색상(byte) * 투영도(%)
            byte a = (byte)(byte.MaxValue * transparency);

            return new Vec4b(b, g, r, a);
        }

    }
}