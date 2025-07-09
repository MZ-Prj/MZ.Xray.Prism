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

namespace MZ.Dashboard.ViewModels
{
    public class XrayRealtimeViewModel : MZBindableBase
    {
        #region Service
        private readonly IXrayService _xrayService;
        #endregion

        #region Params
        private Canvas _canvasImageView;
        public Canvas CanvasImageView { get => _canvasImageView; set => SetProperty(ref _canvasImageView, value); }

        private Canvas _canvasPredictView;
        public Canvas CanvasPredictView { get => _canvasPredictView; set => SetProperty(ref _canvasPredictView, value); }

        public ObservableCollection<IconButtonModel> VideoButtons { get; } = [];

        public MediaProcesser _media;
        public MediaProcesser Media { get => _media; set => SetProperty(ref _media, value); }

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

        #endregion
        public XrayRealtimeViewModel(IContainerExtension container, IXrayService xrayService) : base(container)
        {
            _xrayService = xrayService;

            Media = _xrayService.Media;
            
            base.Initialize();
        }

        public override void InitializeModel()
        {
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Pin), PickerCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.Play), PlayStopCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronDoubleLeft), PreviousCommand));
            VideoButtons.Add(new(nameof(PackIconMaterialKind.ChevronDoubleRight), NextCommand));
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
            _xrayService.Media.PrevNextSlider(-1);
        }

        private void NextButton()
        {
            _xrayService.Media.PrevNextSlider(+1);
        }

        private void ToggleVideoButton(ICommand targetCommand, string iconOn, string iconOff)
        {
            var button = VideoButtons.FirstOrDefault(vb => vb.Command == targetCommand);
            if (button != null)
            {
                button.IconKind = button.IconKind == iconOff ? iconOn : iconOff;
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