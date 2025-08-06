using System;
using System.Collections.Generic;
using OpenCvSharp;
using static MZ.Vision.VisionEnums;

namespace MZ.Vision
{
    /// <summary>
    /// LUT 곡선 함수 모델 클래스
    /// </summary>
    public class VisionLUTModel
    {
        public FunctionNameEnumTypes Name { get; set; }
        public List<double> Parameters { get; set; } = [];
        public List<Point> MinMax { get; set; } = [];
    }

    public class VisionEnums
    {
        /// <summary>
        /// LUT에서 지원하는 함수명 Enum
        /// </summary>
        public enum FunctionNameEnumTypes
        {
            Base,
            Atanh,
            Log,
            Pow,
        }
    }

    /// <summary>
    /// LUT 곡선 함수별 계산
    /// </summary>
    public class VisionLUT
    {
        /// <summary>
        /// 함수 타입별 LUT 값 계산
        /// </summary>
        public static double Run(FunctionNameEnumTypes func, double input, double[] values)
        {
            double result = 0;
            switch (func)
            {
                case FunctionNameEnumTypes.Base:
                    result = Base(input, values);
                    break;
                case FunctionNameEnumTypes.Atanh:
                    result = Atanh(input, values);
                    break;
                case FunctionNameEnumTypes.Log:
                    result = Log(input, values);
                    break;
                case FunctionNameEnumTypes.Pow:
                    result = Pow(input, values);
                    break;
            }
            return result;
        }


        /// <summary>
        /// 역쌍곡선(Atanh) 함수 적용
        /// </summary>
        public static double Atanh(double input, double[] values)
        {
            double result = 0.0;
            try
            {
                double a = values[0];
                double b = values[1];
                double c = values[2];
                double d = values[3];

                double convergence = -4;
                double divergence = 4;

                result = a * Math.Log((b + input) / (c - input)) + d;
                if (result < convergence)
                {
                    result = convergence;
                }
                if (result > divergence)
                {
                    result = divergence;
                }

                result = (result - convergence) / (divergence - convergence);
                result = Math.Max(0, Math.Min(1.0, result));

            }
            catch
            {
                result = 0.0;
            }

            return ErrorCheck(result);
        }

        /// <summary>
        /// 선형 함수 (y = a(x + b) + c)
        /// </summary>
        public static double Base(double input, double[] values)
        {
            double a = values[0];
            double b = values[1];
            double c = values[2];

            double result = a * (input + b) + c;
            return result;
        }

        /// <summary>
        /// 로그 함수 (log<sub>a</sub>(1 + input))
        /// </summary>
        private static double Log(double input, double[] values)
        {
            double a = values[0];
            double result = Math.Log(1 + input) / Math.Log(a);
            return ErrorCheck(result);
        }

        /// <summary>
        /// 거듭제곱 함수 (input^a)
        /// </summary>
        private static double Pow(double input, double[] values)
        {
            double a = values[0];
            double result = Math.Pow(input, a);
            return ErrorCheck(result);
        }

        /// <summary>
        /// 결과값 에러 처리(NaN/Infinity → 0 또는 1)
        /// </summary>
        private static double ErrorCheck(double input)
        {
            if (double.IsNaN(input))
            {
                input = 1;
            }
            if (double.IsNegativeInfinity(input))
            {
                input = 0;
            }
            if (double.IsPositiveInfinity(input))
            {
                input = 1;
            }
            return input;
        }
    }
}
