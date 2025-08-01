using Microsoft.Win32;
using MZ.DTO;
using MZ.Core;
using MZ.Xray.Engine;
using MZ.Util;
using MZ.Domain.Enums;
using MZ.Domain.Models;
using MZ.Resource;
using MZ.AI.Engine;
using MZ.WindowDialog;
using MZ.Dashboard.Views;
using MahApps.Metro.IconPacks;
using Prism.Ioc;
using Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using static MZ.Core.MZModel;
using static MZ.Event.MZEvent;

namespace MZ.Dashboard.ViewModels
{
    public class XrayRealtimeViewModel : MZBindableBase
    {
        #region Service
        private readonly IXrayService _xrayService;
        private readonly IAIService _aiService;
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

        #endregion
        public XrayRealtimeViewModel(IContainerExtension container, IXrayService xrayService, IAIService aiService, IWindowDialogService windowDialogService) : base(container)
        {
            _xrayService = xrayService;
            _aiService = aiService;
            _windowDialogService = windowDialogService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Pin), PickerCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Stop), PlayStopCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronLeft), PreviousCommand, isVisibility: false));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronRight), NextCommand, isVisibility: false));

            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyMinus), ZoomOutCommand, name:UserSettingButtonKeys.ZoomOutButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyPlus), ZoomInCommand, name: UserSettingButtonKeys.ZoomInButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaG), ColorCommand, new SolidColorBrush(Colors.Gray), uid:ColorRole.Gray, name: UserSettingButtonKeys.GrayButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaC), ColorCommand, MZBrush.CreateHsvRadialGradientBrush(), uid: ColorRole.Color, name: UserSettingButtonKeys.ColorButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaR), ColorCommand, new SolidColorBrush(Colors.Orange), uid: ColorRole.Organic, name: UserSettingButtonKeys.OrganicButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaG), ColorCommand, new SolidColorBrush(Colors.Green), uid: ColorRole.Inorganic, name: UserSettingButtonKeys.InorganicButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaB), ColorCommand, new SolidColorBrush(Colors.DodgerBlue), uid: ColorRole.Metal, name: UserSettingButtonKeys.MetalButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Brightness4), BrightDownCommand, name: UserSettingButtonKeys.BrightDownButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Brightness5), BrightUpCommand, name: UserSettingButtonKeys.BrightUpButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.CircleOpacity), ContrastDownCommand, name: UserSettingButtonKeys.ContrastDownButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.CircleHalfFull), ContrastUpCommand, name: UserSettingButtonKeys.ContrastUpButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.FilterRemove), FilterClearCommand, name: UserSettingButtonKeys.FilterClearButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaZ), ZeffectCommand, new SolidColorBrush(Colors.YellowGreen), name: UserSettingButtonKeys.ZeffectButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.HeadRemoveOutline), AIOnOffCommand, name: UserSettingButtonKeys.AIOnOffButton));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.MonitorScreenshot), SaveImageCommand, name: UserSettingButtonKeys.SaveImageButton));

            EtcButtons.Add(new(nameof(PackIconMaterialKind.Cog), SettingCommand, ThemeService.GetResource("MahApps.Brushes.Accent4")));
        }


        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<DashboardNavigationEvent>().Subscribe((NavigationModel model) =>
            {
                Load(model.View == MZViewNames.DashboardControlView);
            }, ThreadOption.UIThread, true);
        }

        #region Button

        private void SettingButton()
        {
            _dialogService.ShowDialog(
                "DashboardFooterButtonControlView",
                new DialogParameters
                {
                    {"Title",  MZRegionNames.DashboardFooterButtonControlRegion},
                    {"RegionName", MZRegionNames.DashboardFooterButtonControlRegion},
                    {"ActionButtons", ActionButtons}
                },
                (IDialogResult result) => {
                });

        }

        private void PickerButton()
        {
            //ui
            ToggleFooterButton(PickerCommand, nameof(PackIconMaterialKind.Pin), nameof(PackIconMaterialKind.PinOff), VideoButtons);
        }
        private void PlayStopButton()
        {
            // ui
            ToggleFooterButton(PlayStopCommand, nameof(PackIconMaterialKind.Play), nameof(PackIconMaterialKind.Stop), VideoButtons);

            VisibilityFooterButton(PreviousCommand, _xrayService.IsPlaying(), VideoButtons);
            VisibilityFooterButton(NextCommand, _xrayService.IsPlaying(), VideoButtons);

            // logic
            _xrayService.PlayStop();
            
        }

        private void PreviousButton()
        {
            _xrayService.PrevNextSlider(-1);
        }

        private void NextButton()
        {
            _xrayService.PrevNextSlider(+1);
        }

        private void ZoomOutButton()
        {
            Media.ChangedFilterZoom(-0.1f);
        }

        private void ZoomInButton()
        {
            Media.ChangedFilterZoom(+0.1f);
        }

        private void ColorButton(object color)
        {
            if (color is ColorRole colorRole)
            {
                Media.ChangedFilterColor(colorRole);
            }
        }

        private void BrightDownButton()
        {
            Media.ChangedFilterBrightness(-0.01f);
        }

        private void BrightUpButton()
        {
            Media.ChangedFilterBrightness(+0.01f);
        }

        private void ContrastDownButton()
        {
            Media.ChangedFilterContrast(-0.1f);
        }

        private void ContrastUpButton()
        {
            Media.ChangedFilterContrast(+0.1f);
        }

        private void FilterClearButton()
        {
            Media.ClearFilter();
        }

        private void AIOnOffButton()
        {
            // ui
            ToggleFooterButton(AIOnOffCommand, nameof(PackIconMaterialKind.HeadRemoveOutline), nameof(PackIconMaterialKind.HeadCheckOutline), ActionButtons);

            // logic
            Yolo.ChangedVisibility();
        }

        private async void ZeffectButton()
        {
            await _windowDialogService.ShowWindow(
                title: MZRegionNames.ZeffectControl,
                regionName: nameof(ZeffectControlView),
                isMultiple: false,
                resizeMode: ResizeMode.NoResize,
                width: 480,
                height: 640);
        }

        private void SaveImageButton()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Save Capture Image",
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

        private void Load(bool check)
        {
            if (check)
            {
                // logic
                _xrayService.Stop();
                _xrayService.Play();

                // ui
                ChangeFooterButton(PlayStopCommand, nameof(PackIconMaterialKind.Stop), VideoButtons);

                VisibilityFooterButton(PreviousCommand, false, VideoButtons);
                VisibilityFooterButton(NextCommand, false, VideoButtons);
            }
        }

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



        #region Behavior
        public void CreateMedia(int width, int height)
        {
            Media.Create(width, height);
        }
        #endregion

    }
}