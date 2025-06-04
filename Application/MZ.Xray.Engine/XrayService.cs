using MZ.Vision;
using OpenCvSharp;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;

namespace MZ.Xray.Engine
{
    public class XrayService : BindableBase
    {
        #region Fields & Properties
        private readonly ProcessThread _visionProcess = new();
        private readonly ProcessThread _videoProcess = new();
        #endregion

        #region Vision Algorithm (OpenCV)
        private readonly VisionBase _visionBase = new();
        private readonly VisionLUT _visionLUT = new();
        #endregion

        private MediaEngine _media = new();
        public MediaEngine Media { get => _media; set => SetProperty(ref _media, value); }

        private CalibrationEngine _calibration = new();
        public CalibrationEngine Calibration { get => _calibration; set => SetProperty(ref _calibration, value); }
        
        private MaterialEngine _material = new();
        public MaterialEngine Material { get => _material; set => SetProperty(ref _material, value); }

        private ZeffectEngine _zeffect = new();
        public ZeffectEngine Zeffect { get => _zeffect; set => SetProperty(ref _zeffect, value); }

        public async Task Process(Mat origin)
        {
            // 받아온 값이 유무 확인
            if (_visionBase.IsEmpty(origin))
            {
                return;
            }

            // 이미지 크기 비율 조정
            Mat line = Calibration.AdjustRatio(origin);

            // 이미지 크기 변경시 초기화
            await UpdateOnResizeAsync(line);

            // Update : Gain, Offset 
            if (Calibration.UpdateOnEnergy(line))
            {
                return;
            }

            // Calibration 알고리즘 계산
            (Mat high, _, Mat color, Mat zeff) = Calculation(line);

            // 물체 유무 판단
            if (await Calibration.IsObjectAsync(high))
            {
                await ShiftAsync(line, color, zeff);
                Media.IncreaseCount();
            }
            else
            {
                await SaveAsync();
                Media.ClearCount();
            }
        }

        public (Mat high, Mat low, Mat color, Mat zeff) Calculation(Mat line)
        {
            int width = line.Width;
            int halfHeight = line.Height / 2;

            // 16uc1
            Mat high = _visionBase.Create(halfHeight, width, MatType.CV_16UC1);
            Mat low = _visionBase.Create(halfHeight, width, MatType.CV_16UC1);

            // 8uc4
            Mat color = _visionBase.Create(halfHeight, width, MatType.CV_8UC4);

            // 8uc1
            Mat zeff = _visionBase.Create(halfHeight, width, MatType.CV_8UC1);

            Parallel.For(0, width, x =>
            {
                int k = 0;
                for (int y = 0; y < halfHeight; y++)
                {
                    int l = halfHeight + y;

                    // Gain/Offset 픽셀 값
                    double gainHigh = Calibration.GetGainPixel(y, 0);
                    double offsetHigh = Calibration.GetOffsetPixel(y, 0);
                    double gainLow = Calibration.GetGainPixel(l, 0);
                    double offsetLow = Calibration.GetGainPixel(l, 0);

                    // Gain값이 최소치 이상일때
                    if (Calibration.CompareBoundaryArtifact(gainHigh))
                    {
                        // 실제 픽셀값 변환
                        double _high = Calibration.Normalize(line.At<ushort>(y, x), offsetHigh, gainHigh, Material.GetHighLowRate());
                        double _low = Calibration.Normalize(line.At<ushort>(l, x), offsetLow, gainLow, Material.GetHighLowRate());

                        // Min, Max 검증
                        ushort _uhigh = (ushort)(Math.Max(_high, _low) * ushort.MaxValue);
                        ushort _ulow = (ushort)(Math.Min(_high, _low) * ushort.MaxValue);

                        // 값 입력
                        high.Set(k, x, _uhigh);
                        low.Set(k, x, _ulow);

                        // zeff 계산
                        zeff.Set(k, x, Zeffect.Calculation(_uhigh, _ulow));

                        // 색상 계산
                        color.Set(k, x, Material.Calculation(_high, _low));

                        k++;
                    }
                }
            });

            return (high, low, color, zeff);
        }

        public async Task UpdateOnResizeAsync(Mat line)
        {
            int width = Media.GetWidth();
            int maxImageWidth = Calibration.GetMaxImageWidth();

            await Task.WhenAll(
                Task.Run( ()=> Calibration.UpdateOnResize(line, width)),
                Task.Run( ()=> Media.UpdateOnResize(line, maxImageWidth)),
                Task.Run( ()=> Zeffect.UpdateOnResize(line, width, maxImageWidth)));
        }

        public async Task ShiftAsync(Mat line, Mat color, Mat zeff)
        {
            await Task.WhenAll(
                Task.Run(() => Calibration.Shift(line)),
                Task.Run(() => Media.Shift(color)),
                Task.Run(() => Zeffect.Shift(zeff)));
        }

        public async Task SaveAsync()
        {

        }
    }
}
