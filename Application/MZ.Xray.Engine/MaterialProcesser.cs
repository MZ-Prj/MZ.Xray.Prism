using MZ.Model;
using MZ.Vision;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static MZ.Vision.VisionEnums;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// 물성 정보 이미지 처리 프로세스
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

        public ObservableCollection<MaterialControlModel> Controls
        {
            get => Model.Controls;
            set => Model.Controls = value;
        }
         
        #endregion

        public MaterialProcesser()
        {
            InitializeMaterial();
        }

        /// <summary>
        /// 초기 설정
        /// </summary>
        public void InitializeMaterial()
        {
            Model.Image = VisionBase.Create(byte.MaxValue, byte.MaxValue, MatType.CV_8UC4);
            Model.Image.SetTo(Scalar.All(0));

            InitializeMaterialControls();
            UpdateAllMaterialGraph();
        }

        /// <summary>
        /// DB에서 불러오지 못할 경우 기존 값
        /// </summary>
        public void InitializeMaterialControls()
        {
            Model.Controls.Add(new (UpdateAllMaterialGraph) { Y = 36, XMin = 0, XMax = 255, Scalar = new Scalar(0, 128, 255, 255) });
            Model.Controls.Add(new (UpdateAllMaterialGraph) { Y = 57, XMin = 0, XMax = 255, Scalar = new Scalar(0, 128, 0, 255) });
            Model.Controls.Add(new (UpdateAllMaterialGraph) { Y = 100, XMin = -10, XMax = 255, Scalar = new Scalar(255, 128, 0, 255) });
        }

        /// <summary>
        /// Controls의 모든값에 대한 정보 갱신
        /// </summary>
        public void UpdateAllMaterialGraph()
        {
            if (Model.Image == null)
            {
                Model.Image = VisionBase.Create(byte.MaxValue, byte.MaxValue, MatType.CV_8UC4);
                Model.Image.SetTo(Scalar.All(0));
            }

            Model.Image.SetTo(Scalar.All(0));

            int width = Model.Image.Width;
            int height = Model.Image.Height;

            SortControls();
            foreach (var control in Model.Controls)
            {
                DrawControl(control, width, height);
            }

            if (Model.Blur > 0)
            {
                Model.Image = VisionBase.Blur(Model.Image, (int)Model.Blur);
            }

            // 그래프 보여주는 용도
            var image = GridViewImage(); 

            Model.ImageSource = image.ToBitmapSource();
        }

        /// <summary>
        /// UI에서 ColorMap 보여주기 위한 영상 처리
        /// </summary>
        /// <returns></returns>
        public Mat GridViewImage()
        {
            Mat image = VisionBase.Flip(Model.Image, FlipMode.X);
            Mat result = VisionBase.SplitRow(image, image.Height / 2, image.Height);

            return result;
        }

        /// <summary>
        /// Material Control을 ColorMap에 그림
        /// </summary>
        public void DrawControl(MaterialControlModel control, int width, int height)
        {
            int xMin = (int)control.XMin;
            int xMax = (int)control.XMax;
            double maxY = control.Y;

            Scalar color = control.Scalar;
            color.Val3 = byte.MaxValue;

            double centerX = (xMin + xMax) / 2.0;
            double halfDistance = (xMax - xMin) / 2.0;
            double p = -maxY / (halfDistance * halfDistance);

            List<Point> parabolaPoints = [];

            for (int x = xMin; x <= xMax; x++)
            {
                if (x < 0 || x >= width)
                {
                    continue;
                }

                // y = a(x - centerX)^2 + b
                double y = p * Math.Pow(x - centerX, 2) + maxY;
                int yAxis = Math.Max(0, Math.Min(height - 1, (int)Math.Round(y)));

                parabolaPoints.Add(new Point(x, yAxis));
            }

            // 채우기 위한 polygon
            var polygonPoints = new List<Point>
            {
                new (xMin, 0)
            };
            polygonPoints.AddRange(parabolaPoints);
            polygonPoints.Add(new Point(xMax, 0));

            Cv2.FillPoly(Model.Image, [[.. polygonPoints]], color);
        }

        /// <summary>
        /// Matrial List 목록 정렬 (Y값 큰 순서)
        /// </summary>
        public void SortControls()
        {
            var sortedList = Model.Controls.OrderByDescending(p => p.Y).ToList();
            Model.Controls.Clear();
            foreach (var parabola in sortedList)
            {
                Model.Controls.Add(parabola);
            }
        }

        /// <summary>
        /// Xray 원시(origin) 데이터에서 Color 색상으로 변경
        /// </summary>
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