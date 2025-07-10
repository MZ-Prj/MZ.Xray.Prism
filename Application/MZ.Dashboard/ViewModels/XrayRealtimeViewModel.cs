using MZ.Core;
using MZ.Dashboard.Models;
using MZ.Xray.Engine;
using MZ.Util;
using MahApps.Metro.IconPacks;
using Prism.Commands;
using Prism.Ioc;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MZ.Domain.Enums;

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

        public ObservableCollection<IconButtonModel> VideoButtons { get; } = [];
        public ObservableCollection<IconButtonModel> ColorButtons { get; } = [];
        public ObservableCollection<IconButtonModel> FilterButtons { get; } = [];

        #endregion

        #region Command
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

            ColorButtons.Add(new(nameof(PackIconMaterialKind.FormatPaint), ColorCommand, new SolidColorBrush(Colors.Gray), uid:ColorRole.Gray));
            ColorButtons.Add(new(nameof(PackIconMaterialKind.FormatPaint), ColorCommand, MZBrush.CreateHsvGradientBrush(), uid: ColorRole.Color));
            ColorButtons.Add(new(nameof(PackIconMaterialKind.FormatPaint), ColorCommand, new SolidColorBrush(Colors.Orange), uid: ColorRole.Organic));
            ColorButtons.Add(new(nameof(PackIconMaterialKind.FormatPaint), ColorCommand, new SolidColorBrush(Colors.Green), uid: ColorRole.Inorganic));
            ColorButtons.Add(new(nameof(PackIconMaterialKind.FormatPaint), ColorCommand, new SolidColorBrush(Colors.DodgerBlue), uid: ColorRole.Metal));

            FilterButtons.Add(new(nameof(PackIconMaterialKind.MagnifyMinus),ZoomOutCommand));
            FilterButtons.Add(new(nameof(PackIconMaterialKind.MagnifyPlus),ZoomInCommand));
            FilterButtons.Add(new(nameof(PackIconMaterialKind.Brightness4), BrightDownCommand));
            FilterButtons.Add(new(nameof(PackIconMaterialKind.Brightness5), BrightUpCommand));
            FilterButtons.Add(new(nameof(PackIconMaterialKind.CircleOpacity), ContrastDownCommand));
            FilterButtons.Add(new(nameof(PackIconMaterialKind.CircleHalfFull), ContrastUpCommand));
            FilterButtons.Add(new(nameof(PackIconMaterialKind.FilterRemove), FilterClearCommand));
        }

        #region Button
        private void PickerButton()
        {
            //ui
            ToggleVideoButton(PickerCommand, nameof(PackIconMaterialKind.Pin), nameof(PackIconMaterialKind.PinOff));
        }
        private void PlayStopButton()
        {
            // ui
            ToggleVideoButton(PlayStopCommand, nameof(PackIconMaterialKind.Play), nameof(PackIconMaterialKind.Stop));

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
            Media.ChangedFilterZoom(-0.05f);
        }

        private void ZoomInButton()
        {
            Media.ChangedFilterZoom(+0.05f);
        }

        private void BrightDownButton()
        {
            Media.ChangedFilterBrightness(-0.05f);
        }

        private void BrightUpButton()
        {
            Media.ChangedFilterBrightness(+0.05f);
        }

        private void ContrastDownButton()
        {
            Media.ChangedFilterContrast(-0.05f);
        }

        private void ContrastUpButton()
        {
            Media.ChangedFilterContrast(+0.05f);
        }

        private void FilterClearButton()
        {
            Media.ClearFilter();
        }

        private void ToggleVideoButton(ICommand targetCommand, string iconOn, string iconOff)
        {
            var button = VideoButtons.FirstOrDefault(vb => vb.Command == targetCommand);
            if (button != null)
            {
                button.IconKind = button.IconKind == iconOff ? iconOn : iconOff;
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