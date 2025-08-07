using Microsoft.Win32;
using MZ.DTO;
using MZ.Core;
using MZ.Xray.Engine;
using MZ.Util;
using MZ.Domain.Enums;
using MZ.Model;
using MZ.Resource;
using MZ.AI.Engine;
using MZ.WindowDialog;
using MZ.Dashboard.Views;
using MZ.Infrastructure;
using MahApps.Metro.IconPacks;
using Prism.Ioc;
using Prism.Events;
using Prism.Commands;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using static MZ.Core.MZModel;
using static MZ.Event.MZEvent;
using System;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// Xray Realtime ViewModel : Xray 실시간 화면 & 제어
    /// </summary>
    public class XrayRealtimeViewModel : MZBindableBase
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

        private Canvas _canvasZeffectView;
        public Canvas CanvasZeffectView { get => _canvasZeffectView; set => SetProperty(ref _canvasZeffectView, value); }

        private Canvas _canvasPredictView;
        public Canvas CanvasPredictView { get => _canvasPredictView; set => SetProperty(ref _canvasPredictView, value); }

        public ObservableCollection<IconButtonModel> VideoButtons { get; } = [];
        public ObservableCollection<IconButtonModel> EtcButtons { get; } = [];
        public ObservableCollection<IconButtonModel> ActionButtons => _xrayService.UI.ActionButtons;
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

        #endregion
        public XrayRealtimeViewModel(IContainerExtension container, IXrayService xrayService, IAIService aiService, IWindowDialogService windowDialogService, IDatabaseService databaseService) : base(container)
        {
            _xrayService = xrayService;
            _aiService = aiService;
            _windowDialogService = windowDialogService;
            _databaseService = databaseService;

            base.Initialize();
        }

        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<DashboardNavigationEvent>().Subscribe((NavigationModel model) =>
            {
                Load(model.View == MZViewNames.DashboardControlView);
            }, ThreadOption.UIThread, true);

        }

        #region Button

        /// <summary>
        /// 설정 버튼 : 버튼 보여질 유무
        /// DashboardFooterButtonControlView에 값 전달
        /// </summary>
        private async void SettingButton()
        {
            await _windowDialogService.ShowWindow(
                title: LanguageService.GetString($"Lng{MZRegionNames.DashboardFooterButtonControlRegion}"),
                regionName: nameof(DashboardFooterButtonControlView),
                parameters : new Prism.Regions.NavigationParameters{
                    { "ActionButtons", ActionButtons }
                },
                isMultiple: false,
                resizeMode: ResizeMode.NoResize,
                width: 240,
                height: 220);
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
            ToggleFooterButton(PlayStopCommand, nameof(PackIconMaterialKind.Play), nameof(PackIconMaterialKind.Stop), VideoButtons);

            VisibilityFooterButton(PreviousCommand, _xrayService.IsPlaying(), VideoButtons);
            VisibilityFooterButton(NextCommand, _xrayService.IsPlaying(), VideoButtons);

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
        private void ZoomOutButton()
        {
            Media.ChangedFilterZoom(-0.1f);
        }

        /// <summary>
        /// 확대
        /// </summary>
        private void ZoomInButton()
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
                title: LanguageService.GetString($"Lng{MZRegionNames.ZeffectControl}"),
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
                Title = LanguageService.GetString($"Lng{MZResourceNames.SavePDF}"),
                Filter = "PNG  (*.png)|*.png",
                DefaultExt = ".png"
            };

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                XrayDataSaveManager.Base(Media.ChangedScreenToMat(), saveFileDialog.FileName);
            }
        }

        #endregion

        /// <summary>
        /// 뷰 로드 시 기본 상태 초기화
        /// </summary>
        /// <param name="check">bool</param>
        private async void Load(bool check)
        {
            if (check)
            {
                // ui
                CreateButtons();
                ChangeFooterButton(PlayStopCommand, nameof(PackIconMaterialKind.Stop), VideoButtons);

                VisibilityFooterButton(PreviousCommand, false, VideoButtons);
                VisibilityFooterButton(NextCommand, false, VideoButtons);

                // logic
                _xrayService.Stop();
                _xrayService.Play();

                var userSetting = await _databaseService.User.GetUserSetting();
                if (userSetting.Success)
                {
                    _xrayService.UI.LoadActionButton(userSetting.Data?.Buttons);
                }


            }
        }

        /// <summary>
        /// 버튼 집합
        /// </summary>
        private void CreateButtons()
        {
            //ui
            VideoButtons.Clear();
            ActionButtons.Clear();
            EtcButtons.Clear();

            VideoButtons.Add(new(nameof(PackIconMaterialKind.Pin), PickerCommand, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Picker)));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Stop), PlayStopCommand, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_PlayStop)));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronLeft), PreviousCommand, isVisibility: false, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Previous)));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronRight), NextCommand, isVisibility: false, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Next)));

            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyMinus), ZoomOutCommand, name: UserSettingButtonKeys.ZoomOutButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_ZoomOut)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyPlus), ZoomInCommand, name: UserSettingButtonKeys.ZoomInButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_ZoomIn)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaG), ColorCommand, new SolidColorBrush(Colors.Gray), uid: ColorRole.Gray, name: UserSettingButtonKeys.GrayButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Gray)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaC), ColorCommand, MZBrush.CreateHsvRadialGradientBrush(), uid: ColorRole.Color, name: UserSettingButtonKeys.ColorButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Color)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaR), ColorCommand, new SolidColorBrush(Colors.Orange), uid: ColorRole.Organic, name: UserSettingButtonKeys.OrganicButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Organic)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaG), ColorCommand, new SolidColorBrush(Colors.Green), uid: ColorRole.Inorganic, name: UserSettingButtonKeys.InorganicButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Inorganic)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaB), ColorCommand, new SolidColorBrush(Colors.DodgerBlue), uid: ColorRole.Metal, name: UserSettingButtonKeys.MetalButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Metal)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Brightness4), BrightDownCommand, name: UserSettingButtonKeys.BrightDownButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_BrightDown)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Brightness5), BrightUpCommand, name: UserSettingButtonKeys.BrightUpButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_BrighUp)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.CircleOpacity), ContrastDownCommand, name: UserSettingButtonKeys.ContrastDownButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_ContrastDown)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.CircleHalfFull), ContrastUpCommand, name: UserSettingButtonKeys.ContrastUpButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_ContrastUp)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.FilterRemove), FilterClearCommand, name: UserSettingButtonKeys.FilterClearButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_FilterClear)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaZ), ZeffectCommand, new SolidColorBrush(Colors.YellowGreen), name: UserSettingButtonKeys.ZeffectButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Zeffect)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.HeadRemoveOutline), AIOnOffCommand, name: UserSettingButtonKeys.AIOnOffButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_AIOnOff)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.MonitorScreenshot), SaveImageCommand, name: UserSettingButtonKeys.SaveImageButton, tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_SaveImage)));

            EtcButtons.Add(new(nameof(PackIconMaterialKind.Cog), SettingCommand, ThemeService.GetResource("MahApps.Brushes.Accent4"), tooltip: MZResourceNames.AddLng(MZResourceNames.XrayRealtimeRegion_Setting)));
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