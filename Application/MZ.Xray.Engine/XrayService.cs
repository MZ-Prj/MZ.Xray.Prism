using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading.Channels;
using Prism.Mvvm;
using Prism.Events;
using MZ.Core;
using MZ.Logger;
using MZ.Vision;
using MZ.Model;
using MZ.Infrastructure;
using MZ.AI.Engine;
using MZ.DTO;
using MZ.Util;
using MZ.Resource;
using OpenCvSharp;
using Microsoft.Extensions.Configuration;
using static MZ.Event.MZEvent;
using System.Collections.Generic;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// XrayService는 엑스레이 영상 처리와 관련된 주요 비즈니스 로직을 담당
    /// UI와 DB, AI 엔진, 이벤트 등 다양한 서비스와 연동되어, 영상 수신부터 후처리, UI 반영까지 전체 흐름을 관리
    /// </summary>
    public partial class XrayService : BindableBase, IXrayService
    {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// 앱 전체 설정 정보를 접근하기 위한 Configuration.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// event aggregator
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// DB 관련 비즈니스 로직(유저, AI 옵션 등)에 접근하는 데이터 서비스
        /// </summary>
        private readonly IDatabaseService _databaseService;
        /// <summary>
        /// AI 엔진 서비스 (Yolo 등 객체 인식 및 AI 연동)
        /// </summary>
        private readonly IAIService _aiService;

        /// <summary>
        /// 비동기 영상 처리 파이프라인을 위한 Channel
        /// </summary>
        private readonly Channel<Mat> _imageProcessingChannel = Channel.CreateBounded<Mat>(100);

        public XrayService(IConfiguration configuration, IEventAggregator eventAggregator, IDatabaseService databaseService, IAIService aIService)
        {
            _configuration = configuration;
            _eventAggregator = eventAggregator;
            _databaseService = databaseService;
            _aiService = aIService;

            // 네트워크 수신 담당 프로세서 생성
            _socketReceive = new SocketReceiveProcesser(_eventAggregator);

            InitializeEvent();
            InitializeProcess();
        }

        /// <summary>
        /// 이벤트 초기화
        /// Xray 장비 등에서 이미지 수신 시점에 DB 로그인 여부와 시스템 상태를 판단하여 비동기 영상 큐에 등록
        /// </summary>
        public void InitializeEvent()
        {
            try
            {
                _eventAggregator.GetEvent<FileReceiveEvent>().Subscribe((FileModel file) =>
                {
                    if (IsRunning && _databaseService.User.IsLoggedIn())
                    {
                        _imageProcessingChannel.Writer.TryWrite(file.Image);
                    }
                }, ThreadOption.UIThread, true);
            }
            catch (Exception ex) 
            { 
                MZLogger.Error(ex.ToString()); 
            }
        }

        /// <summary>
        /// 영상 처리 파이프라인 초기화 (비동기 Queue 기반 영상 처리 수행)
        /// </summary>
        public void InitializeProcess()
        {
            Task.Run(ProcessImagesFromChannel);
        }

        /// <summary>
        /// 채널에 쌓인 영상(Mat)을 순차적으로 꺼내서 Process() 메서드에 전달
        /// </summary>
        private async Task ProcessImagesFromChannel()
        {
            await foreach (var image in _imageProcessingChannel.Reader.ReadAllAsync())
            {
                await Process(image);
            }
        }
    }

    public partial class XrayService : BindableBase, IXrayService
    {
        #region Fields & Properties
        /// <summary>
        /// 화면(Screen) 갱신용 Task를 중지/제어하기 위한 CancellationTokenSource.
        /// </summary>
        private CancellationTokenSource _screenCts;

        private Task _screenTask;
        private bool IsRunning => _screenCts != null && !_screenCts.IsCancellationRequested;

        /// <summary>
        /// 실시간 화면 랜더링을 위한 갱신용 FPS (실로직용)
        /// comment -> Model.Inforamtion.FPS는 화면 갱신될때 값을 표기(ui용)
        /// </summary>
        private int _fps = 60;

        #endregion

        /// <summary>
        /// 화면(Screen) 갱신 주기 Task 시작 (비동기)
        /// 프레임 속도(FPS)마다 UpdateScreen() 호출하여 영상 프레임, AI, Zeffect, 슬라이더 처리 등 수행
        /// </summary>
        private void StartScreenTask()
        {
            if (IsRunning)
            {
                return;
            }

            _screenCts = new CancellationTokenSource();
            _screenTask = ScreenLoopAsync(_screenCts.Token);
        }

        private async Task ScreenLoopAsync(CancellationToken token)
        {
            Media.LastestSlider();

            double targetTicks = CalcTargetTicks(_fps);
            long nextTick = Stopwatch.GetTimestamp();

            while (!token.IsCancellationRequested)
            {
                long frameStart = Stopwatch.GetTimestamp();

                // UI 반영
                await InvokeUpdateScreenAsync(token); 

                // 다음 프레임 기준 계산
                nextTick = GetNextTick(nextTick, targetTicks);

                // 지연 계산 및 대기/동기화
                if (!await WaitUntilAsync(nextTick, token))  
                {
                    //프레임이 늦었을 때, 현재 시간으로 동기화
                    nextTick = Stopwatch.GetTimestamp();
                }

                // 중간에 FPS 변경 시 주기 갱신
                targetTicks = RefreshTargetIfChanged(targetTicks, _fps);
            }
        }

        /// <summary>
        /// UI 스레드에서 UpdateScreen 실행 
        /// </summary>
        private async Task InvokeUpdateScreenAsync(CancellationToken token)
        {
            await _dispatcher.InvokeAsync(
                UpdateScreen,
                DispatcherPriority.Render,
                token
            );
        }


        /// <summary>
        /// FPS를 Stopwatch Tick 변환
        /// </summary>
        private double CalcTargetTicks(int fps)
        {
            return Stopwatch.Frequency / (double)fps;
        }

        /// <summary>
        /// 다음 프레임 기준 시간 계산
        /// </summary>
        private long GetNextTick(long next, double targetTicks)
        {
            return next + (long)targetTicks;
        }

        /// <summary>
        /// 목표 시간까지 대기. 
        /// </summary>
        private async Task<bool> WaitUntilAsync(long tick, CancellationToken token)
        {
            long now = Stopwatch.GetTimestamp();
            long remain = tick - now;

            if (remain <= 0)
            {
                return false;
            }
            int delay = (int)(remain * 1000 / Stopwatch.Frequency);
            if (delay > 0)
            {
                try 
                { 
                    await Task.Delay(delay, token); 
                }
                catch (Exception) {}
            }
            return true;
        }


        /// <summary>
        /// FPS 변경 감지 시 새 주기로 갱신
        /// </summary>
        private double RefreshTargetIfChanged(double current, int fps)
        {
            double target = CalcTargetTicks(fps);
            return Math.Abs(target - current) > 1 ? target : current;
        }

        /// <summary>
        /// 화면 갱신 Task 중지
        /// </summary>
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

        /// <summary>
        /// 영상처리 전체 재생(Play) 시작
        /// 기존 Task 모두 중지 후, Video/Screen Task를 새로 시작
        /// </summary>
        public void Play()
        {
            Stop();

            StartScreenTask();
        }

        /// <summary>
        /// 영상처리 전체 중지(Stop) (화면, 비디오 Task 모두 중지)
        /// </summary>
        public void Stop()
        {
            StopScreenTask();
        }

        /// <summary>
        /// 현재 영상처리(재생) 상태 반환
        /// </summary>
        public bool IsPlaying()
        {
            return IsRunning;
        }

        /// <summary>
        /// 영상 재생/정지 토글
        /// </summary>
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

        /// <summary>
        /// 화면 갱신 및 프레임/슬라이더/AI/Zeffect 등 상태 업데이트
        /// - Media, Zeffect의 이미지 소스를 freeze(스레드 안정화)
        /// - 새 프레임이 필요한 경우, 프레임 추가/삭제 및 AI 객체 인식 결과 반영
        /// - 슬라이더가 끝까지 도달하면 프레임 삭제 및 슬라이더 초기화
        /// - 슬라이더 증분, 프레임 수 증가 등 갱신
        /// </summary>
        
        public async Task UpdateScreen()
        {
            if (IsRunning)
            {
                await Task.WhenAll(Media.FreezeImageSourceAsync(),
                    Zeffect.FreezeImageSourceAsync());

                if (Media.IsCountUpperZero() && Media.IsFrameUpdateRequired())
                {
                    Add();

                    if (Media.CompareSlider())
                    {
                        Remove();

                        Media.ResetSlider();
                    }
                    Media.IncrementSlider();
                }
                Media.IncreaseInterval();
            }
            
        }

        /// <summary>
        /// UI에서 슬라이더(프레임) 인덱스 이동 시, Media/Zeffect/AI 상태 동기화 (버튼)
        /// </summary>
        public void PrevNextSlider(int index)
        {
            int slider = Media.PrevNextSlider(index);

            PrevNextSliderBar(slider);
        }

        /// <summary>
        /// UI에서 슬라이더(프레임) 인덱스 이동 시, Media/Zeffect/AI 상태 동기화 (슬라이드바)
        /// </summary>
        public void PrevNextSliderBar(int index)
        {
            Media.ChangeFrame(index);
            Zeffect.ChangeFrame(index);
            _aiService.ChangeObjectDetections(index);
        }

        private void Add()
        {
            Media.AddFrame();
            Zeffect.AddFrame();
            _aiService.AddObjectDetection();
        }

        private void Remove()
        {
            Media.RemoveFrame();
            Zeffect.RemoveFrame();
            _aiService.RemoveObjectDetection();
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

        /// <summary>
        /// 소켓 통신(네트워크) 초기화 및 데이터 수신 비동기 루프 수행
        /// - 내부적으로 TcpListener 생성 및 지정 포트 Listen
        /// - 클라이언트 연결 시 이미지 데이터를 비동기로 지속 수신(ReceiveAsync)
        /// - 예외 발생 시 로그 기록, UI 중단 없는 백그라운드 동작 
        /// </summary>
        public void InitializeSocket()
        {
            Task.Run(async () =>
            {
                try
                {
                    // 소켓 리스너 초기화
                    SocketReceive.Create();

                    // 비동기 수신 루프
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

        private CurveSplineProcesser _curveSpline = new();
        public CurveSplineProcesser CurveSpline { get => _curveSpline; set => SetProperty(ref _curveSpline, value); }

        #endregion

        /// <summary>
        /// Xray 한 줄(line) 이미지 데이터에 대한 전체 처리(한 줄(line) 데이터를 받아 영상 분석 및 AI/Zeffect/Media 전체 흐름 제어)
        /// - 비어있는 이미지 필터링 (입력값 검증)
        /// - 보정/비율 조정 (Calibration)
        /// - 이미지 크기 변화 감지 후 하위 모듈들 resize 적용 (비동기)
        /// - Gain/Offset 등 에너지 기반 상태 갱신
        /// - 고저 신호분해 및 Color/Zeff 이미지 생성 (Calculation)
        /// - 물체 검출시 - 이동(Shift)/카운트 증가
        /// - 물체 미검출시 - AI예측/저장/카운트 초기화/보정값 업데이트
        /// </summary>
        /// <param name="origin">Mat : 원본 Xray 이미지</param>
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
                    await ShiftAsync(line, CurveSpline.UpdateMat(color), zeff);
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
        /// Process의 순 알고리즘 처리만 적용
        /// UnitTest 코드
        /// </summary>
        public async Task ProcessTest(Mat origin)
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
                await ShiftAsync(line, CurveSpline.UpdateMat(color), zeff);
                Media.IncreaseCount();
            }
            else
            {
                Media.ClearCount();
                Calibration.UpdateGain(line);
            }
        }

        /// <summary>
        /// 한 줄(Line) X-ray 이미지 처리 수행
        /// - 상/하(High/Low) 신호 분해
        /// - 컬러 및 Zeff 효과 계산
        /// - Gain/Offset 기반 정규화 및 값 보정
        /// - 병렬 처리로 연산 효율화
        /// </summary>
        /// <param name="line">Mat : 보정 적용된 Xray 이미지</param>
        /// <returns>Mat : high, low, color, zeff </returns>
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
        /// 입력 라인의 크기 변화시, 각 모듈(Calibration/Media/Zeffect 등)에서 내부 버퍼/상태를 비동기로 Resize 처리
        /// - Resize 처리가 필요한 이유: 이미지 폭 등 변화 시 기존 연산 값(버퍼 등) 재계산 필요
        /// </summary>
        /// <param name="line">Mat : 입력 이미지</param>
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
        /// Xray 프레임 분석에서 객체가 있다고 판단시 Shift 수행
        /// - 각 모듈별(Calibration/Media/Zeff/AI) Shift 연산 및 상태 업데이트를 비동기로 수행
        /// - AI의 경우도 입력 이미지의 Width 기준으로 별도 연산
        /// </summary>
        /// <param name="line">Mat : 보정 적용된 Xray 이미지</param>
        /// <param name="color">Mat : 알고리즘 처리 된 색상 이미지</param>
        /// <param name="zeff">Mat : 알고리즘 처리 된 Zeffect</param>
        /// <returns></returns>
        public async Task ShiftAsync(Mat line, Mat color, Mat zeff)
        {

            int width = color.Width;
            await Task.WhenAll(
                Calibration.ShiftAsync(line),
                Media.ShiftAsync(color),
                Zeffect.ShiftAsync(zeff),
                _aiService.ShiftAsync(width));
        }

        /// <summary>
        /// 처리된 Xray 정보를 파일 및 DB에 저장
        /// - 저장 경로/이름 생성(날짜 등)
        /// - 프레임별로 이미지/원본/화면 캡쳐/DB연동/AI 예측 결과 모두 비동기로 저장
        /// - 저장 후, 데이터베이스 및 파일로 각각 보존
        /// </summary>
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

            // 저장은 UI 처리와 별계여야함으로 백그라운드에서 수행
            Task.Run(async () =>
            {
                await Task.WhenAll(
                    XrayDataSaveManager.ImageAsync(Media.Image, start, end, path, $"{time}.png"),
                    XrayDataSaveManager.OriginAsync(Calibration.Origin, Calibration.Offset, Calibration.Gain, start, end, path, $"{time}.tiff"),
                    XrayDataSaveManager.ScreenAsync(Media.ChangedScreenToMat(), path, $"{time}.png"),
                    _databaseService.Image.Save(new ImageSaveRequest(path, $"{time}.png", (end - start), height, ObjectDetectionMapper.ModelsToEntities(_aiService.Yolo.ChangePositionCanvasToMat()))),
                    _aiService.Save(path, $"{time}.json"));
            });

        }
    }

    public partial class XrayService : BindableBase, IXrayService
    {
        /// <summary>
        /// 모든 상태를 DB에 저장
        /// - 각 모듈(보정/필터/재질/효과/AI옵션/유저설정)의 Mapper를 이용해 Request로 변환
        /// - 각 도메인 서비스의 Save 메서드 호출(비동기 실행이 아닌 동기)
        /// - 저장 후 User 로그아웃까지 처리 (로그아웃 후엔 자동 로그인 필요)
        /// </summary>
        public void SaveDatabase()
        {
            _databaseService.Calibration.Save(XrayVisionCalibrationMapper.ModelToRequest(Calibration.Model));
            _databaseService.Filter.Save(XrayVisionFilterMapper.ModelToRequest(Media.Model.Filter));
            _databaseService.Material.Save(XrayVisionMaterialMapper.ModelToRequest(Material.Model));
            _databaseService.ZeffectControl.Save(XrayVisionZeffectControlMapper.ModelToRequest(Zeffect.Model.Controls));
            _databaseService.CurveControl.Save(XrayVisionCurveControlMapper.ModelToRequest(CurveSpline.Points));

            _databaseService.AIOption.Save(CategoryMapper.ModelToRequest(_aiService.Yolo.Categories));

            _databaseService.User.SaveUserSetting(UserSettingMapper.ModelToRequest(ThemeService.CurrentTheme,LanguageService.CurrentLanguage, UI.ActionButtons));

            _databaseService.User.Logout();
        }

        /// <summary>
        /// DB에서 사용자별 데이터(보정, 필터, 재질, 효과 등)를 읽어와 각 모듈의 Model 적용
        /// - 로그인된 사용자의 이름을 기준으로 각 데이터 Load
        /// - 매핑(Mapper) 클래스로 Entity→Model 변환 후 할당
        /// - 비동기(await)로 순차적 로딩, 성공시 해당 Model 업데이트
        /// </summary>
        public async void LoadDatabase()
        {
            // Current Username
            var currentUser = _databaseService.User.CurrentUser();
            string username = currentUser.Data;

            // Calibration
            var calibration = await _databaseService.Calibration.Load(new(username));
            if (calibration.Success)
            {
                Calibration.Model = XrayVisionCalibrationMapper.EntityToModel(calibration.Data);
            }

            // Filter
            var filter = await _databaseService.Filter.Load(new(username));
            if (filter.Success)
            {
                Media.Filter = XrayVisionFilterMapper.EntityToModel(filter.Data);
            }

            // Material
            var material = await _databaseService.Material.Load(new(username));
            if (material.Success)
            {
                Material.Model = XrayVisionMaterialMapper.EntityToModel(material.Data, Material.UpdateAllMaterialGraph);
                Material.UpdateAllMaterialGraph();
            }

            // Zeffect Control
            var zeffectControl = await _databaseService.ZeffectControl.Load(new(username));
            if (zeffectControl.Success)
            {
                Zeffect.Model.Controls = [.. XrayVisionZeffectControlMapper.EntitiesToModels(zeffectControl.Data)];
                Zeffect.UpdateZeffectControl();
            }

            // Curve Control
            var curveControl = await _databaseService.CurveControl.Load(new(username));
            if (curveControl.Success)
            {
                CurveSpline.Points = [.. XrayVisionCurveControlMapper.EntitiesToModels(curveControl.Data)];
            }
        }
    }

    public partial class XrayService : BindableBase, IXrayService
    {
        /// <summary>
        /// AI(모델 및 옵션) 초기화 메서드
        /// - AI 파일이 없거나 DB에 옵션 레코드가 없으면 새로 다운로드 및 생성 
        /// - 이미 있으면 DB 및 파일 기준으로 AI를 불러옴
        /// </summary>
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

        /// <summary>
        /// AI 모델 파일이 없거나 옵션 DB가 없을 때 초기 설정
        /// - 기존 파일 삭제 및 폴더 정리  
        /// - 모델 파일 다운로드  
        /// - AI 서비스 생성 및 옵션 DB 저장
        /// </summary>
        private async Task InitializeCreateAI(string path, string link)
        {
            MZWebDownload download = new();

            MZIO.TryDeleteFile(path);
            MZIO.TryMakeDirectoryRemoveFile(path);

            await _databaseService.AIOption.Delete();

            UI.SplashMessage = MZRegionNames.GetLanguage(MZRegionNames.SplashRegionAIDownloadStart);
            bool checkDownload = await download.RunAsync(link, path);

            if (checkDownload)
            {
                UI.SplashMessage = MZRegionNames.GetLanguage(MZRegionNames.SplashRegionAILoad);

                _aiService.Create(path);
                _aiService.Dispose();

                UI.SplashMessage = MZRegionNames.GetLanguage(MZRegionNames.SplashRegionAIDatabaseUpdate);
                var aiOption = await _databaseService.AIOption.Create(_aiService.YoloToRequest());
                if (aiOption.Success)
                {
                    _aiService.Load(aiOption.Data);
                    UI.SplashMessage = MZRegionNames.GetLanguage(MZRegionNames.SplashRegionSuccess);
                }
                UI.SplashMessage = MZRegionNames.GetLanguage(MZRegionNames.SplashRegionFail);

            }
        }

        /// <summary>
        /// DB 및 파일이 존재하는 경우 초기 설정
        /// - DB에서 옵션 로드  
        /// - 파일이 있으면 AI 모델 생성
        /// - 없으면 재다운로드 및 재생성
        /// </summary>
        private async Task InitializeLoadAI(string path, string link)
        {
            UI.SplashMessage = MZRegionNames.GetLanguage(MZRegionNames.SplashRegionAILoad);
            var aiOption = await _databaseService.AIOption.Load();
            if (aiOption.Success)
            {
                bool checkModel = _aiService.IsSavedModel(aiOption.Data.OnnxModel);
                if (checkModel)
                {
                    UI.SplashMessage = MZRegionNames.GetLanguage(MZRegionNames.SplashRegionAIDatabaseLoad);
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

        /// <summary>
        /// 현재 프레임 상태에서 AI 객체 감지 예측을 수행
        /// - 프레임 존재 여부
        /// - 예측 영역(Mat) 추출
        /// - AI 서비스에 예측 요청
        /// </summary>
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

                double canvasWidth = (Media.Information.Width * mat.Width / Media.Image.Width);
                double canvasHeigth = Media.Information.Height;

                _aiService.Predict(mat.ToMemoryStream(), new(mat.Width, mat.Height) , new(canvasWidth, canvasHeigth), (Media.Image.Width - mat.Width));

            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
            }
        }
    }

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
