using System;
using MZ.Vision;
using MZ.Model;
using OpenCvSharp;
using Prism.Mvvm;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using OpenCvSharp.WpfExtensions;
using static MZ.Vision.VisionEnums;
using System.Windows.Threading;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// 물성에 대한 정보를 수치로 표기
    /// </summary>
    public class ZeffectProcesser : BindableBase
    {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        #region Params
        private ZeffectModel _model = new();
        public ZeffectModel Model { get => _model; set => SetProperty(ref _model, value); }

        public ObservableCollection<ZeffectControlModel> Controls
        {
            get => Model.Controls;
            set => Model.Controls = value;
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

        public ObservableCollection<FrameModel> Frames
        {
            get => Model.Frames;
            set => Model.Frames = value;
        }

        #endregion

        public ZeffectProcesser()
        {
            InitializeZeffectControls();
        }

        /// <summary>
        /// 초기 설정
        /// </summary>
        public void InitializeZeffectControls()
        {
            Model.Controls.Add(new () { Content = "None", Color= Color.FromArgb(0, 0, 0, 0), Min = 0.0, Max = 0.0, Check = true});
            Model.Controls.Add(new () { Content = "Organic", Color = Color.FromArgb(128, 255, 128, 128), Min = 0.0, Max = 0.3 });
            Model.Controls.Add(new () { Content = "Inorganic", Color = Color.FromArgb(128, 0, 128, 0), Min = 0.3, Max = 0.75 });
            Model.Controls.Add(new () { Content = "Metal", Color = Color.FromArgb(128, 0, 128, 255), Min = 0.75, Max = 1.0 });

            Model.Control = Model.Controls.First();
        }

        /// <summary>
        /// Zeffect Controls값중 선택된것 갱신
        /// </summary>
        public void UpdateZeffectControl()
        {
            Model.Control = Controls.FirstOrDefault(c => c.Check);
        }

        /// <summary>
        /// Zeffect 계산
        /// </summary>
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

        /// <summary>
        /// 켄버스(UI) 크기가 변경 되었을 경우 이미지 크기 갱신
        /// </summary>
        public void UpdateOnResize(Mat line, int width, int maxImageWidth)
        {
            if (width != maxImageWidth)
            {
                Image = VisionBase.Create((line.Height / 2), maxImageWidth, MatType.CV_8UC1, new Scalar(0));
            }
        }
        /// <summary>
        /// 현제 프레임 추가
        /// </summary>
        public void AddFrame()
        {
            Frames.Add(new() { Image = Image, DateTime = DateTime.Now });
        }
        /// <summary>
        /// 가장 첫번째 Frame 제거
        /// </summary>
        public void RemoveFrame()
        {
            Frames.RemoveAt(0);
        }
        /// <summary>
        /// ImageSource 갱신
        /// </summary>
        public void ChangeFrame(int index)
        {
            try
            {
                if (index <= 0 || index > Frames.Count)
                {
                    return;
                }
                ImageSource = VisionBase.CanFreezeImageSource(Frames[index - 1].Image.ToBitmapSource());

            }
            catch { }
        }
        /// <summary>
        /// 켄버스(UI) 크기가 변경 되었을 경우 이미지 크기 갱신 (비동기)
        /// </summary>
        public async Task UpdateOnResizeAsync(Mat line, int width, int maxImageWidth)
        {
            await Task.Run(() =>
            {
                UpdateOnResize(line, width, maxImageWidth);
            });
        }
        /// <summary>
        /// 이미지 Shift 수행
        /// </summary>
        public void Shift(Mat zeff)
        {
            Image = VisionBase.ShiftCol(Model.Image, zeff);
        }
        /// <summary>
        /// 이미지 Shift 수행 (비동기)
        /// </summary>
        public async Task ShiftAsync(Mat zeff)
        {
            await Task.Run(() =>
            {
                Shift(zeff);
            });
        }
        /// <summary>
        /// Mat -> ImageSource로 변환
        /// </summary>
        public void FreezeImageSource()
        {
            _dispatcher.InvokeAsync(() =>
            {
                ImageSource = VisionBase.ToBitmapSource(Image, ref Model.ImageSourceWriteableBitmap);
            }, DispatcherPriority.Render);
        }
        /// <summary>
        /// Mat -> ImageSource로 변환 (비동기)
        /// </summary>
        public async Task FreezeImageSourceAsync()
        {
            await Task.Run(FreezeImageSource);
        }
    }
}
