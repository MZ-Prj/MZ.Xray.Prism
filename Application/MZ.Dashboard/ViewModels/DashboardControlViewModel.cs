using MahApps.Metro.IconPacks;
using MZ.AI.Engine;
using MZ.Core;
using MZ.Dashboard.Views;
using MZ.Domain.Enums;
using MZ.DTO;
using MZ.Infrastructure;
using MZ.Model;
using MZ.Resource;
using MZ.Util;
using MZ.WindowDialog;
using MZ.Xray.Engine;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using Microsoft.Win32;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// Dashboard Control ViewModel : 메인 화면을 보여주기 위한 중간 담당 계층
    /// </summary>
    public class DashboardControlViewModel : MZBindableBase
    {
        #region Service
        private readonly IXrayService _xrayService;
        private readonly IAIService _aiService;
        private readonly IDatabaseService _databaseService;
        private readonly IWindowDialogService _windowDialogService;

        public MediaProcesser Media
        {
            get => _xrayService.Media;
            set => _xrayService.Media = value;
        }

        public CalibrationProcesser Calibration
        {
            get => _xrayService.Calibration;
            set => _xrayService.Calibration = value;
        }

        public ZeffectProcesser Zeffect
        {
            get => _xrayService.Zeffect;
            set => _xrayService.Zeffect = value;
        }

        public YoloProcessor Yolo
        {
            get => _aiService.Yolo;
            set => _aiService.Yolo = value;
        }

        public ObservableCollection<IconButtonModel> ActionButtons
        {
            get => _xrayService.UI.ActionButtons;
            set => _xrayService.UI.ActionButtons = value;
        }


        #endregion

        #region Params
        private Canvas _canvasImageView;
        public Canvas CanvasImageView
        {
            get => _canvasImageView;
            set
            {
                if (SetProperty(ref _canvasImageView, value))
                {
                    Media.Screen = value;
                }
            }
        }

        private ObservableCollection<IconButtonModel> _videoButtons = [];
        public ObservableCollection<IconButtonModel> VideoButtons { get => _videoButtons; set => SetProperty(ref _videoButtons, value); }

        private ObservableCollection<IconButtonModel> _etcButtons = [];
        public ObservableCollection<IconButtonModel> EtcButtons { get => _etcButtons; set => SetProperty(ref _etcButtons, value); }

        private bool _isRunning;
        public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }
        #endregion

        #region Command
        private DelegateCommand _settingCommand;
        public ICommand SettingCommand => _settingCommand ??= new(MZAction.Wrapper(SettingButton));

        private DelegateCommand _pickerCommand;
        public ICommand PickerCommand => _pickerCommand ??= new(MZAction.Wrapper(PickerButton));

        private DelegateCommand _playStopCommand;
        public ICommand PlayStopCommand => _playStopCommand ??= new(MZAction.Wrapper(PlayStopButton));

        private DelegateCommand _previousCommand;
        public ICommand PreviousCommand => _previousCommand ??= new(MZAction.Wrapper(PreviousButton));

        private DelegateCommand _nextCommand;
        public ICommand NextCommand => _nextCommand ??= new(MZAction.Wrapper(NextButton));

        private DelegateCommand<object> _colorCommand;
        public ICommand ColorCommand => _colorCommand ??= new(MZAction.Wrapper<object>(ColorButton));

        private DelegateCommand _zoomInCommand;
        public ICommand ZoomInCommand => _zoomInCommand ??= new(MZAction.Wrapper(ZoomInButton));

        private DelegateCommand _zoomOutCommand;
        public ICommand ZoomOutCommand => _zoomOutCommand ??= new(MZAction.Wrapper(ZoomOutButton));

        private DelegateCommand _brightUpCommand;
        public ICommand BrightUpCommand => _brightUpCommand ??= new(MZAction.Wrapper(BrightUpButton));

        private DelegateCommand _brightDownCommand;
        public ICommand BrightDownCommand => _brightDownCommand ??= new(MZAction.Wrapper(BrightDownButton));

        private DelegateCommand _contrastUpCommand;
        public ICommand ContrastUpCommand => _contrastUpCommand ??= new(MZAction.Wrapper(ContrastUpButton));

        private DelegateCommand _contrastDownCommand;
        public ICommand ContrastDownCommand => _contrastDownCommand ??= new(MZAction.Wrapper(ContrastDownButton));

        private DelegateCommand _filterClearCommand;
        public ICommand FilterClearCommand => _filterClearCommand ??= new(MZAction.Wrapper(FilterClearButton));

        private DelegateCommand _aiOnOffCommand;
        public ICommand AIOnOffCommand => _aiOnOffCommand ??= new(MZAction.Wrapper(AIOnOffButton));

        private DelegateCommand _zeffectCommand;
        public ICommand ZeffectCommand => _zeffectCommand ??= new(MZAction.Wrapper(ZeffectButton));

        private DelegateCommand _saveImageCommand;
        public ICommand SaveImageCommand => _saveImageCommand ??= new(MZAction.Wrapper(SaveImageButton));

        private DelegateCommand _changedSliderCommand;
        public ICommand ChangedSliderCommand => _changedSliderCommand ??= new DelegateCommand(MZAction.Wrapper(ChangedSlider, false));

        private DelegateCommand _relativeWidthRatioUpCommand;
        public ICommand RelativeWidthRatioUpCommand => _relativeWidthRatioUpCommand ??= new(MZAction.Wrapper(RelativeWidthRatioUpButton));

        private DelegateCommand _relativeWidthRatioDownCommand;
        public ICommand RelativeWidthRatioDownCommand => _relativeWidthRatioDownCommand ??= new(MZAction.Wrapper(RelativeWidthRatioDownButton));


        #endregion


        public DashboardControlViewModel(IContainerExtension container, IXrayService xrayService, IAIService aiService, IWindowDialogService windowDialogService, IDatabaseService databaseService) : base(container)
        {
            _xrayService = xrayService;
            _aiService = aiService;
            _windowDialogService = windowDialogService;
            _databaseService = databaseService;

            base.Initialize();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Load();
        }


        #region Button

        /// <summary>
        /// 설정 버튼 : 버튼 보여질 유무
        /// DashboardFooterButtonControlView에 값 전달
        /// </summary>
        private async void SettingButton()
        {
            await _windowDialogService.ShowWindow(
                title: LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.DashboardFooterButtonControlRegion)),
                regionName: nameof(DashboardFooterButtonControlView),
                parameters: new NavigationParameters{
                    { "ActionButtons", ActionButtons }
                },
                resizeMode: ResizeMode.NoResize,
                isMultiple: false,
                width: 228,
                height: 364);

        }

        /// <summary>
        /// Pin 토글
        /// </summary>
        private void PickerButton()
        {
            // ui
            ToggleFooterButton(PickerCommand, nameof(PackIconMaterialKind.Pin), nameof(PackIconMaterialKind.PinOff), VideoButtons);
        }

        /// <summary>
        /// 재생/정지 토글
        /// </summary>
        private void PlayStopButton()
        {

            // ui
            IsRunning = _xrayService.IsPlaying();

            ToggleFooterButton(PlayStopCommand, nameof(PackIconMaterialKind.Play), nameof(PackIconMaterialKind.Stop), VideoButtons);

            ChangedVisibility(IsRunning);


            // logic
            _xrayService.PlayStop();

        }

        /// <summary>
        /// 이전 영상 : 프레임 단위
        /// </summary>
        private void PreviousButton()
        {
            _xrayService.PrevNextSlider(-1);
        }

        /// <summary>
        /// 다음 영상 : 프레임 단위
        /// </summary>
        private void NextButton()
        {
            _xrayService.PrevNextSlider(+1);
        }

        /// <summary>
        /// 축소
        /// </summary>
        public void ZoomOutButton()
        {
            Media.ChangedFilterZoom(-0.1f);
        }

        /// <summary>
        /// 확대
        /// </summary>
        public void ZoomInButton()
        {
            Media.ChangedFilterZoom(+0.1f);
        }

        /// <summary>
        /// 색상 변경
        /// </summary>
        /// <param name="color">ColorRole</param>
        private void ColorButton(object color)
        {
            if (color is ColorRole colorRole)
            {
                Media.ChangedFilterColor(colorRole);
            }
        }

        /// <summary>
        /// 밝기 감소
        /// </summary>
        private void BrightDownButton()
        {
            Media.ChangedFilterBrightness(-0.01f);
        }

        /// <summary>
        /// 밝기 증가
        /// </summary>
        private void BrightUpButton()
        {
            Media.ChangedFilterBrightness(+0.01f);
        }

        /// <summary>
        /// 명암 감소
        /// </summary>
        private void ContrastDownButton()
        {
            Media.ChangedFilterContrast(-0.1f);
        }

        /// <summary>
        /// 명암 증가
        /// </summary>
        private void ContrastUpButton()
        {
            Media.ChangedFilterContrast(+0.1f);
        }

        /// <summary>
        /// 필터 초기화
        /// </summary>
        private void FilterClearButton()
        {
            Media.ClearFilter();
        }

        /// <summary>
        /// 인공지능 탐지 토글
        /// </summary>
        private void AIOnOffButton()
        {
            // ui
            ToggleFooterButton(AIOnOffCommand, nameof(PackIconMaterialKind.HeadRemoveOutline), nameof(PackIconMaterialKind.HeadCheckOutline), ActionButtons);

            // logic
            Yolo.ChangedVisibility();
        }

        /// <summary>
        /// Zeffect 화면
        /// </summary>
        private async void ZeffectButton()
        {
            await _windowDialogService.ShowWindow(
                title: LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.ZeffectControl)),
                regionName: nameof(ZeffectControlView),
                isMultiple: false,
                resizeMode: ResizeMode.NoResize,
                width: 480,
                height: 640);
        }

        /// <summary>
        /// 화면 캡쳐 저장
        /// </summary>
        private void SaveImageButton()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = LanguageService.GetString(MZRegionNames.AddLng(MZRegionNames.SaveScreen)),
                Filter = "PNG  (*.png)|*.png",
                DefaultExt = ".png"
            };

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                XrayDataSaveManager.Base(Media.ChangedScreenToMat(), saveFileDialog.FileName);
            }
        }


        private void RelativeWidthRatioUpButton()
        {
            Calibration.ChangedRelativeWidthRatio(-0.1);
        }

        private void RelativeWidthRatioDownButton()
        {
            Calibration.ChangedRelativeWidthRatio(+0.1);
        }

        #endregion

        /// <summary>
        /// 뷰 로드 시 기본 상태 초기화
        /// </summary>
        /// <param name="check">bool</param>
        private async void Load()
        {
            // ui
            CreateButtons();
            ChangeFooterButton(PlayStopCommand, nameof(PackIconMaterialKind.Stop), VideoButtons);

            ChangedVisibility(false);

            // logic
            _xrayService.Stop();
            _xrayService.Play();
            
            // db
            var userSetting = await _databaseService.User.GetUserSetting();
            if (userSetting.Success)
            {
                _xrayService.UI.LoadActionButton(userSetting.Data?.Buttons);
            }
        }

        /// <summary>
        /// 버튼 집합
        /// </summary>
        private void CreateButtons()
        {
            VideoButtons.DisposeAndClear();
            ActionButtons.DisposeAndClear();
            EtcButtons.DisposeAndClear();

            //ui
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Pin), PickerCommand, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionPicker)));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Stop), PlayStopCommand, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionPlayStop)));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronLeft), PreviousCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionPrevious)));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronRight), NextCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionNext)));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ArrowCollapseHorizontal), RelativeWidthRatioDownCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionRelativeWidthRatioDown)));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ArrowExpandHorizontal), RelativeWidthRatioUpCommand, isVisibility: false, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionRelativeWidthRatioUp)));


            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyMinus), ZoomOutCommand, name: UserSettingButtonKeys.ZoomOutButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionZoomOut)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyPlus), ZoomInCommand, name: UserSettingButtonKeys.ZoomInButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionZoomIn)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaG), ColorCommand, new SolidColorBrush(Colors.Gray), uid: ColorRole.Gray, name: UserSettingButtonKeys.GrayButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionGray)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaC), ColorCommand, MZBrush.CreateHsvRadialGradientBrush(), uid: ColorRole.Color, name: UserSettingButtonKeys.ColorButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionColor)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaR), ColorCommand, new SolidColorBrush(Colors.Orange), uid: ColorRole.Organic, name: UserSettingButtonKeys.OrganicButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionOrganic)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaG), ColorCommand, new SolidColorBrush(Colors.Green), uid: ColorRole.Inorganic, name: UserSettingButtonKeys.InorganicButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionInorganic)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaB), ColorCommand, new SolidColorBrush(Colors.DodgerBlue), uid: ColorRole.Metal, name: UserSettingButtonKeys.MetalButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionMetal)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Brightness4), BrightDownCommand, name: UserSettingButtonKeys.BrightDownButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionBrightDown)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Brightness5), BrightUpCommand, name: UserSettingButtonKeys.BrightUpButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionBrighUp)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.CircleOpacity), ContrastDownCommand, name: UserSettingButtonKeys.ContrastDownButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionContrastDown)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.CircleHalfFull), ContrastUpCommand, name: UserSettingButtonKeys.ContrastUpButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionContrastUp)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.FilterRemove), FilterClearCommand, name: UserSettingButtonKeys.FilterClearButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionFilterClear)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaZ), ZeffectCommand, new SolidColorBrush(Colors.YellowGreen), name: UserSettingButtonKeys.ZeffectButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionZeffect)));


            ActionButtons.Add(new(nameof(PackIconMaterialKind.HeadRemoveOutline), AIOnOffCommand, name: UserSettingButtonKeys.AIOnOffButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionAIOnOff)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.MonitorScreenshot), SaveImageCommand, name: UserSettingButtonKeys.SaveImageButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionSaveImage)));

            EtcButtons.Add(new(nameof(PackIconMaterialKind.Cog), SettingCommand, ThemeService.GetResource("MahApps.Brushes.Accent4"), tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegionSetting)));
        }


        /// <summary>
        /// 버튼 아이콘 변경
        /// </summary>
        /// <param name="targetCommand">ICommand</param>
        /// <param name="icon">string</param>
        /// <param name="buttonCollections">ObservableCollection<IconButtonModel>[]</param>
        private void ChangeFooterButton(ICommand targetCommand, string icon, params ObservableCollection<IconButtonModel>[] buttonCollections)
        {
            foreach (var collection in buttonCollections)
            {
                foreach (var button in collection)
                {
                    if (button.Command == targetCommand)
                    {
                        button.IconKind = icon;
                    }
                }
            }
        }



        /// <summary>
        /// 버튼 아이콘 On/Off 토글
        /// </summary>
        /// <param name="targetCommand">ICommand</param>
        /// <param name="iconOn">string</param>
        /// <param name="iconOff">string</param>
        /// <param name="buttonCollections">ObservableCollection<IconButtonModel>[]</param>
        private void ToggleFooterButton(ICommand targetCommand, string iconOn, string iconOff, params ObservableCollection<IconButtonModel>[] buttonCollections)
        {
            foreach (var collection in buttonCollections)
            {
                foreach (var button in collection)
                {
                    if (button.Command == targetCommand)
                    {
                        button.IconKind = button.IconKind == iconOff ? iconOn : iconOff;
                    }
                }
            }
        }

        /// <summary>
        /// 버튼 노출상태 변경
        /// </summary>
        /// <param name="targetCommand">ICommand</param>
        /// <param name="isVisibility">bool</param>
        /// <param name="buttonCollections">ObservableCollection<IconButtonModel>[]</param>
        private void VisibilityFooterButton(ICommand targetCommand, bool isVisibility, params ObservableCollection<IconButtonModel>[] buttonCollections)
        {
            foreach (var collection in buttonCollections)
            {
                foreach (var button in collection)
                {
                    if (button.Command == targetCommand)
                    {
                        button.IsVisibility = isVisibility;
                    }
                }
            }
        }

        /// <summary>
        /// Slider Event(ScrollChanged) 
        /// 마우스로 슬라이더 제어시 사용
        /// </summary>
        private void ChangedSlider()
        {
            _xrayService.PrevNextSliderBar(Media.Information.Slider);
        }


        /// <summary>
        /// Play/Stop일때 적용시킬 Visibility
        /// </summary>
        /// <param name="check">bool</param>
        private void ChangedVisibility(bool check)
        {
            VisibilityFooterButton(PreviousCommand, check, VideoButtons);
            VisibilityFooterButton(NextCommand, check, VideoButtons);

            VisibilityFooterButton(RelativeWidthRatioDownCommand, check, VideoButtons);
            VisibilityFooterButton(RelativeWidthRatioUpCommand, check, VideoButtons);

        }


        #region Behavior
        /// <summary>
        /// Behavior에서 지정할 초기 가로/세로
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="height">heightparam>
        public void CreateMedia(int width, int height)
        {
            Media.Create(width, height);
        }

        #endregion

    }

}

