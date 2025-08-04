using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading.Channels;
using Prism.Mvvm;
using Prism.Events;
using MZ.Logger;
using MZ.Vision;
using MZ.Domain.Models;
using MZ.Infrastructure;
using MZ.AI.Engine;
using MZ.DTO;
using MZ.Util;
using MZ.Resource;
using OpenCvSharp;
using Microsoft.Extensions.Configuration;
using static MZ.Event.MZEvent;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XrayService : BindableBase, IXrayService
    {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        private readonly IConfiguration _configuration;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDatabaseService _databaseService;
        private readonly IAIService _aiService;

        private readonly Channel<Mat> _imageProcessingChannel = Channel.CreateBounded<Mat>(100);

        public XrayService(IConfiguration configuration, IEventAggregator eventAggregator, IDatabaseService databaseService, IAIService aIService)
        {
            _configuration = configuration;
            _eventAggregator = eventAggregator;
            _databaseService = databaseService;
            _aiService = aIService;

            _socketReceive = new SocketReceiveProcesser(_eventAggregator);

            InitializeEvent();
            InitializeProcess();
        }

        public void InitializeEvent()
        {
            _eventAggregator.GetEvent<FileReceiveEvent>().Subscribe((FileModel file) =>
            {
                if (IsRunning && _databaseService.User.IsLoggedIn())
                {
                    _imageProcessingChannel.Writer.TryWrite(file.Image);
                }
            }, ThreadOption.UIThread, true);

        }

        public void InitializeProcess()
        {
            Task.Run(ProcessImagesFromChannel);
        }

        private async Task ProcessImagesFromChannel()
        {
            await foreach (var image in _imageProcessingChannel.Reader.ReadAllAsync())
            {
                await Process(image);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class XrayService : BindableBase, IXrayService
    {
        #region Fields & Properties
        private CancellationTokenSource _screenCts;
        private CancellationTokenSource _videoCts;

        private Task _screenTask;
        private Task _videoTask;
        private bool IsRunning => _screenCts != null && !_screenCts.IsCancellationRequested;
        #endregion

        private void StartVideoTask()
        {
            if (IsRunning)
            {
                return;
            }

            _videoCts = new();

            _videoTask = Task.Run(async () =>
            {
                try
                {
                    while (!_videoCts.Token.IsCancellationRequested)
                    {
                        await Task.Delay(1000 * Media.Information.VideoDelay, _videoCts.Token);
                        Media.SaveVideo();
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    MZLogger.Error(ex.ToString());
                }
            }, _videoCts.Token);
        }

        private void StartScreenTask()
        {
            if (IsRunning)
            {
                return;
            }

            _screenCts = new();

            _screenTask = Task.Run(async () =>
            {
                try
                {
                    Media.LastestSlider();

                    while (!_screenCts.Token.IsCancellationRequested)
                    {
                        await UpdateScreen();
                        await Task.Delay(1000 / Media.Information.FPS, _screenCts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    MZLogger.Error(ex.ToString());
                }
            }, _screenCts.Token);
        }

        private void StopVideoTask()
        {
            if (!IsRunning)
            {
                return;
            }

            _videoCts?.Cancel();
            _videoTask.Wait(TimeSpan.FromMilliseconds(100));

            _videoCts?.Dispose();
            _videoCts = null;
            _videoTask = null;
        }

        private void StopScreenTask()
        {
            if (!IsRunning)
            {
                return;
            }

            _screenCts?.Cancel();
            _screenTask.Wait(TimeSpan.FromMilliseconds(100));

            _screenCts?.Dispose();
            _screenCts = null;
            _screenTask = null;
        }

        public void Play()
        {
            Stop();

            StartVideoTask();
            StartScreenTask();
        }

        public void Stop()
        {
            StopVideoTask();
            StopScreenTask();
        }

        public bool IsPlaying()
        {
            return IsRunning;
        }

        public void PlayStop()
        {
            if (IsPlaying())
            {
                Stop();
            }
            else
            {
                Play();
            }
        }

        public async Task UpdateScreen()
        {
            await Task.WhenAll(
                 Media.FreezeImageSourceAsync(),
                 Zeffect.FreezeImageSourceAsync());
           
            if (Media.IsFrameUpdateRequired())
            {
                Media.AddFrame();
                Zeffect.AddFrame();

                _aiService.AddObjectDetection();

                if (Media.Information.Slider >= Media.Information.MaxSlider)
                {
                    Media.RemoveFrame();
                    Zeffect.RemoveFrame();

                    _aiService.RemoveObjectDetection();

                    Media.ResetSlider();
                }
                Media.IncrementSlider();
            }
            Media.IncreaseInterval();
        }

        public void PrevNextSlider(int index)
        {
            int slider = Media.PrevNextSlider(index);

            Media.ChangeFrame(slider);
            Zeffect.ChangeFrame(slider);
            _aiService.ChangeObjectDetections(slider);
        }

        public void ClearScreen()
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public async Task Process(Mat origin)
        {
            try
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
                    Predict();
                    Save();

                    Media.ClearCount();
                    Calibration.UpdateGain(line);
                }
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
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
                Zeffect.ShiftAsync(zeff),
                _aiService.ShiftAsync(line.Width));
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
            int height = Media.Image.Height;

            if (frameCount <= 0)
            {
                return;
            }

            string path = XrayDataSaveManager.GetPath();
            string time = XrayDataSaveManager.GetCurrentTime();

            (int start, int end) = XrayDataSaveManager.GetSplitPosition(width, sensorWidth, frameCount);

            Task.Run(async () =>
            {
                await Task.WhenAll(
                    XrayDataSaveManager.ImageAsync(Media.Image, start, end, path, $"{time}.png"),
                    XrayDataSaveManager.OriginAsync(Calibration.Origin, Calibration.Offset, Calibration.Gain, start, end, path, $"{time}.tiff"),
                    XrayDataSaveManager.ScreenAsync(Media.ChangedScreenToMat(), path, $"{time}.png"),
                    _databaseService.Image.Save(new ImageSaveRequest(path, $"{time}.png", (end - start), height, ObjectDetectionMapper.ModelsToEntities(_aiService.Yolo.ObjectDetections))),
                    _aiService.Save(path, $"{time}.json"));
            });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class XrayService : BindableBase, IXrayService
    {
        public void SaveDatabase()
        {
            _databaseService.Calibration.Save(XrayVisionCalibrationMapper.ModelToRequest(Calibration.Model));
            _databaseService.Filter.Save(XrayVisionFilterMapper.ModelToRequest(Media.Model.Filter));
            _databaseService.Material.Save(XrayVisionMaterialMapper.ModelToRequest(Material.Model));
            _databaseService.ZeffectControl.Save(XrayVisionZeffectControlMapper.ModelToRequest(Zeffect.Model.Controls));

            _databaseService.AIOption.Save(CategoryMapper.ModelToRequest(_aiService.Yolo.Categories));

            _databaseService.User.SaveUserSetting(UserSettingMapper.ModelToRequest(ThemeService.CurrentTheme,LanguageService.CurrentLanguage, UI.ActionButtons));

            _databaseService.User.Logout();
        }

        public async void LoadDatabase()
        {
            //current username
            var currentUser = _databaseService.User.CurrentUser();
            string username = currentUser.Data;

            //calibration
            var calibration = await _databaseService.Calibration.Load(new(username));
            if (calibration.Success)
            {
                Calibration.Model = XrayVisionCalibrationMapper.EntityToModel(calibration.Data);
            }

            //filter
            var filter = await _databaseService.Filter.Load(new(username));
            if (filter.Success)
            {
                Media.Filter = XrayVisionFilterMapper.EntityToModel(filter.Data);
            }

            //material
            var material = await _databaseService.Material.Load(new(username));
            if (material.Success)
            {
                Material.Model = XrayVisionMaterialMapper.EntityToModel(material.Data, Material.UpdateAllMaterialGraph);
                Material.UpdateAllMaterialGraph();
            }

            //zeffect control
            var zeffectControl = await _databaseService.ZeffectControl.Load(new(username));
            if (zeffectControl.Success)
            {
                Zeffect.Model.Controls = [.. XrayVisionZeffectControlMapper.EntitiesToModels(zeffectControl.Data)];
                Zeffect.UpdateZeffectControl();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class XrayService : BindableBase, IXrayService
    {
        public async Task InitializeAI()
        {
            string aiPath = _configuration["AI:Path"];
            string aiDownloadLink = _configuration["AI:DownloadLink"];

            bool checkModel = _aiService.IsSavedModel(aiPath);

            var existOneRecord = await _databaseService.AIOption.ExistOneRecord();
            bool checkDatabase = existOneRecord.Success;

            if (!(checkModel && checkDatabase))
            {
                await InitializeCreateAI(aiPath, aiDownloadLink);
            }
            else
            {
                await InitializeLoadAI(aiPath, aiDownloadLink);
            }
        }

        private async Task InitializeCreateAI(string path, string link)
        {
            MZWebDownload download = new();

            MZIO.TryDeleteFile(path);
            MZIO.TryMakeDirectoryRemoveFile(path);

            await _databaseService.AIOption.Delete();

            bool checkDownload = await download.RunAsync(link, path);

            if (checkDownload)
            {
                _aiService.Create(path);
                _aiService.Dispose();

                var aiOption = await _databaseService.AIOption.Create(_aiService.YoloToRequest());
                if (aiOption.Success)
                {
                    _aiService.Load(aiOption.Data);
                }
            }
        }

        private async Task InitializeLoadAI(string path, string link)
        {
            var aiOption = await _databaseService.AIOption.Load();
            if (aiOption.Success)
            {
                bool checkModel = _aiService.IsSavedModel(aiOption.Data.OnnxModel);
                if (checkModel)
                {
                    _aiService.Load(aiOption.Data);
                }
                else
                {
                    await _databaseService.AIOption.Delete();
                    await InitializeCreateAI(path, link);
                }
            }
            else
            {
                await InitializeCreateAI(path, link);
            }
        }

        private void Predict()
        {
            int frameCount = Media.Information.Count;
            int sensorWidth = Calibration.SensorImageWidth;
            int width = Media.Image.Width;

            if (frameCount <= 0)
            {
                return;
            }

            try
            {
                (int start, int end) = XrayDataSaveManager.GetSplitPosition(width, sensorWidth, frameCount);

                var mat = VisionBase.SplitCol(VisionBase.BlendWithBackground(Media.Image), start, end);

                _aiService.Predict(mat.ToMemoryStream(), new(mat.Width, mat.Height) , new((Media.Information.Width* mat.Width/Media.Image.Width), Media.Information.Height), (Media.Image.Width- mat.Width));

            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
            }
        }
    }

    /// <summary>
    /// UI
    /// </summary>
    public partial class XrayService : BindableBase, IXrayService
    {
        private UIProcesser _ui = new();
        public UIProcesser UI { get => _ui; set => SetProperty(ref _ui, value); }
    }

    public partial class XrayService : BindableBase, IXrayService
    {
        public PDFProcesser PDF { get; set; } = new();


    }
}
