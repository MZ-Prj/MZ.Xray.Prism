using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Prism.Mvvm;
using MZ.Logger;
using MZ.Vision;
using MZ.Domain.Models;
using OpenCvSharp;
using Prism.Events;
using static MZ.Event.MZEvent;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MZ.Xray.Engine
{
    public partial class XrayService : BindableBase, IXrayService
    {
        public readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private readonly IEventAggregator _eventAggregator;

        public XrayService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _socketReceive = new SocketReceiveProcesser(_eventAggregator);

            InitializeEvent();
        }

        public void InitializeEvent()
        {
            _eventAggregator.GetEvent<FileReceiveEvent>().Subscribe( async (FileModel file) =>
            {
                await Process(file.Image);
            }, ThreadOption.UIThread, true);
        }
    }

    public partial class XrayService : BindableBase, IXrayService
    {
        #region Fields & Properties
        private readonly ProcessThread _mediaProcess = new();
        #endregion

        private void StartMediaProcess()
        {
            _mediaProcess.IsRunning = true;
            _mediaProcess.Thread = new Thread(() =>
            {
                try
                {
                    while (_mediaProcess.IsRunning)
                    {
                        MediaProcess();
                        Thread.Sleep((1000 / Media.Information.FPS));
                    }
                }
                catch (Exception ex)
                {
                    MZLogger.Error(ex.ToString());
                }

            });
            _mediaProcess.Thread.Start();
        }


        private void StopMediaProcess()
        {
            _mediaProcess.IsRunning = false;
            if (_mediaProcess.Thread != null && _mediaProcess.Thread.IsAlive)
            {
                _mediaProcess.Thread.Join(_mediaProcess.LazyTime);
            }
        }


        public void Play()
        {
            Stop();

            StartMediaProcess();
        }

        public void Stop()
        {
            StopMediaProcess();
        }

        private void MediaProcess()
        {
            _dispatcher.Invoke(() =>
            {
                
            });
        }

    }

    public partial class XrayService : BindableBase, IXrayService
    {
        #region Processer (네트워크)
        private SocketReceiveProcesser _socketReceive;
        public SocketReceiveProcesser SocketReceive { get => _socketReceive; set => SetProperty(ref _socketReceive, value); }
        #endregion

        public void InitializeSocket()
        {
            Task.Run(async () =>
            {
                try
                {
                    SocketReceive.Create();
                    await SocketReceive.ReceiveAsync();
                }
                catch (Exception ex)
                {
                    MZLogger.Error(ex.ToString());
                }
            });
        }

    }

    public partial class XrayService : BindableBase, IXrayService
    {

        #region Processer (모델 및 알고리즘 처리)
        private MediaProcesser _media = new();
        public MediaProcesser Media { get => _media; set => SetProperty(ref _media, value); }

        private CalibrationProcesser _calibration = new();
        public CalibrationProcesser Calibration { get => _calibration; set => SetProperty(ref _calibration, value); }
        
        private MaterialProcesser _material = new();
        public MaterialProcesser Material { get => _material; set => SetProperty(ref _material, value); }

        private ZeffectProcesser _zeffect = new();
        public ZeffectProcesser Zeffect { get => _zeffect; set => SetProperty(ref _zeffect, value); }

        #endregion

        #region Manager (관리)
        private XrayDataSaveManager _saveManager = new();
        public XrayDataSaveManager SaveManager { get => _saveManager; set => SetProperty(ref _saveManager, value); }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public async Task Process(Mat origin)
        {
            if (VisionBase.IsEmpty(origin))
            {
                return;
            }

            // 이미지 크기 비율 조정
            Mat line = Calibration.AdjustRatio(origin);

            // 이미지 크기 변경시 초기화
            await UpdateOnResizeAsync(line);

            // Update : Gain, Offset 
            if (!Calibration.UpdateOnEnergy(line))
            {
                return;
            }

            // Calibration 알고리즘 계산
            (Mat high, _, Mat color, Mat zeff) = Calculation(line);

            // 물체 유무 판단
            bool isObject = await Calibration.IsObjectAsync(high);

            if (isObject)
            {
                await ShiftAsync(line, color, zeff);
                Media.IncreaseCount();
            }
            else
            {
                Save();
                Media.ClearCount();
                Calibration.UpdateGain(line);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public (Mat high, Mat low, Mat color, Mat zeff) Calculation(Mat line)
        {
            int width = line.Width;
            int halfHeight = line.Height / 2;

            // 16uc1
            Mat high = VisionBase.Create(halfHeight, width, MatType.CV_16UC1);
            Mat low = VisionBase.Create(halfHeight, width, MatType.CV_16UC1);

            // 8uc4
            Mat color = VisionBase.Create(halfHeight, width, MatType.CV_8UC4);

            // 8uc1
            Mat zeff = VisionBase.Create(halfHeight, width, MatType.CV_8UC1);

            //
            Mat gain = Calibration.Gain;
            Mat offset = Calibration.Offset;

            Parallel.For(0, width, x =>
            {
                int k = 0;
                for (int y = 0; y < halfHeight; y++)
                {
                    int l = halfHeight + y;

                    // Gain(High,Low) / Offset(High,Low) 픽셀 값
                    double gl = gain.At<ushort>(y, 0);
                    double ol = offset.At<ushort>(y, 0);
                    double gh = gain.At<ushort>(l, 0);
                    double oh = offset.At<ushort>(l, 0);

                    // Gain값이 해당 boundary 이상일 때
                    if (Calibration.CompareBoundaryArtifact(gh))
                    {
                        // 정규화
                        double nh = Calibration.Normalize(line.At<ushort>(l, x), oh, gh, Material.HighLowRate);
                        double nl = Calibration.Normalize(line.At<ushort>(y, x), ol, gl, Material.HighLowRate);

                        // Min, Max 검증
                        ushort uh = (ushort)(Math.Max(nh, nl) * ushort.MaxValue);
                        ushort ul = (ushort)(Math.Min(nh, nl) * ushort.MaxValue);

                        // 값 입력
                        high.Set(k, x, uh);
                        low.Set(k, x, ul);
                        // zeff 계산
                        zeff.Set(k, x, Zeffect.Calculation(uh, ul));
                        // 색상 계산
                        color.Set(k, x, Material.Calculation(nh, nl));

                        k++;
                    }
                }
            });

            return (high, low, color, zeff);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public async Task UpdateOnResizeAsync(Mat line)
        {
            int width = Media.Image.Width;
            int maxImageWidth = Calibration.MaxImageWidth;

            await Task.WhenAll(
                Calibration.UpdateOnResizeAsync(line, width),
                Media.UpdateOnResizeAsync(line, maxImageWidth),
                Zeffect.UpdateOnResizeAsync(line, width, maxImageWidth));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="color"></param>
        /// <param name="zeff"></param>
        /// <returns></returns>
        public async Task ShiftAsync(Mat line, Mat color, Mat zeff)
        {
            await Task.WhenAll(
                Calibration.ShiftAsync(line),
                Media.ShiftAsync(color),
                Zeffect.ShiftAsync(zeff));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Save()
        {
            int frameCount = Media.Information.Count;
            int sensorWidth = Calibration.SensorImageWidth;
            int width = Media.Image.Width;

            if (frameCount <= 0)
            {
                return;
            }

            string path = SaveManager.GetPath();
            string time = SaveManager.GetCurrentTime();

            (int start, int end) = SaveManager.GetSplitPosition(width, sensorWidth, frameCount);

            //이미지 저장 background
            Task.Run(async () =>
            {
                await Task.WhenAll(
                    SaveManager.ImageAsync(Media.Image, start, end, path, $"{time}.png"),
                    SaveManager.OriginAsync(Calibration.Origin, Calibration.Offset, Calibration.Gain, start, end, path, $"{time}.tiff"));
            });
        }
    }
}
