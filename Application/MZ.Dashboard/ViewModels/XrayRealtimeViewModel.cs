using MZ.Core;
using MZ.Xray.Engine;
using MZ.Util;
using MZ.Domain.Enums;
using MZ.Resource;
using MZ.Domain.Models;
using MahApps.Metro.IconPacks;
using Prism.Commands;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MZ.Dashboard.ViewModels
{
    public class XrayRealtimeViewModel : MZBindableBase
    {
        #region Service
        private readonly IXrayService _xrayService;

        public MediaProcesser Media
        {
            get => _xrayService.Media;
            set => _xrayService.Media = value;
        }

        #endregion

        #region Params
        private Canvas _canvasImageView;
        public Canvas CanvasImageView { get => _canvasImageView; set => SetProperty(ref _canvasImageView, value); }

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
        public XrayRealtimeViewModel(IContainerExtension container, IXrayService xrayService) : base(container)
        {
            _xrayService = xrayService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Pin), PickerCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Play), PlayStopCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronLeft), PreviousCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronRight), NextCommand));

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
            Media.PrevNextSlider(-1);
        }

        private void NextButton()
        {
            Media.PrevNextSlider(+1);
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
            Media.ChangedFilterBrightness(-0.1f);
        }

        private void BrightUpButton()
        {
            Media.ChangedFilterBrightness(+0.1f);
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
        }

        private void SaveImageButton()
        {
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


        private void ColorButton(object color)
        {
            if (color is ColorRole colorRole)
            {
                _xrayService.Media.ChangedFilterColor(colorRole);
            }
        }

        #endregion

        #region Service
        public void CreateMedia(int width, int height)
        {
            _xrayService.Media.Create(width, height);
        }
        #endregion

    }
}