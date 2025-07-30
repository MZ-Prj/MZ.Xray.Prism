using MZ.Core;
using MZ.Xray.Engine;
using MZ.Util;
using MZ.Domain.Enums;
using MZ.Domain.Models;
using MZ.Resource;
using MZ.AI.Engine;
using MahApps.Metro.IconPacks;
using Prism.Commands;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace MZ.Dashboard.ViewModels
{
    public class XrayRealtimeViewModel : MZBindableBase
    {
        #region Service
        private readonly IXrayService _xrayService;
        private readonly IAIService _aiService;

        public MediaProcesser Media
        {
            get => _xrayService.Media;
            set => _xrayService.Media = value;
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

        private Canvas _canvasPredictView;
        public Canvas CanvasPredictView { get => _canvasPredictView; set => SetProperty(ref _canvasPredictView, value); }

        public ObservableCollection<IconButtonModel> ActionButtons { get; } = [];
        public ObservableCollection<IconButtonModel> VideoButtons { get; } = [];
        public ObservableCollection<IconButtonModel> EtcButtons { get; } = [];
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

        private DelegateCommand _saveImageCommand;
        public ICommand SaveImageCommand => _saveImageCommand ??= new(MZAction.Wrapper(SaveImageButton));

        #endregion
        public XrayRealtimeViewModel(IContainerExtension container, IXrayService xrayService, IAIService aiService) : base(container)
        {
            _xrayService = xrayService;
            _aiService = aiService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Pin), PickerCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Play), PlayStopCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronLeft), PreviousCommand, isVisibility: false));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronRight), NextCommand, isVisibility: false));

            EtcButtons.Add(new(nameof(PackIconMaterialKind.Cog), SettingCommand, ThemeService.GetResource("MahApps.Brushes.Accent4")));

            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyMinus), ZoomOutCommand));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyPlus), ZoomInCommand));

            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaG), ColorCommand, new SolidColorBrush(Colors.Gray), uid:ColorRole.Gray));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaC), ColorCommand, MZBrush.CreateHsvRadialGradientBrush(), uid: ColorRole.Color));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaR), ColorCommand, new SolidColorBrush(Colors.Orange), uid: ColorRole.Organic));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaG), ColorCommand, new SolidColorBrush(Colors.Green), uid: ColorRole.Inorganic));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.AlphaB), ColorCommand, new SolidColorBrush(Colors.DodgerBlue), uid: ColorRole.Metal));

            ActionButtons.Add(new(nameof(PackIconMaterialKind.Brightness4), BrightDownCommand));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Brightness5), BrightUpCommand));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.CircleOpacity), ContrastDownCommand));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.CircleHalfFull), ContrastUpCommand));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.FilterRemove), FilterClearCommand));

            ActionButtons.Add(new(nameof(PackIconMaterialKind.HeadRemoveOutline), AIOnOffCommand));

            ActionButtons.Add(new(nameof(PackIconMaterialKind.MonitorScreenshot), SaveImageCommand));

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
            if (_xrayService.IsPlaying())
            {
                _xrayService.Stop();
            }
            else
            {
                _xrayService.Play();
            }
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

        private void ColorButton(object color)
        {
            if (color is ColorRole colorRole)
            {
                Media.ChangedFilterColor(colorRole);
            }
        }

        #endregion

        #region Service
        public void CreateMedia(int width, int height)
        {
            Media.Create(width, height);
        }
        #endregion

    }
}