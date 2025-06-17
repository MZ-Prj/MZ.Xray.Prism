using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Dashboard.Models;
using MZ.Producer.Engine;
using MZ.Sidebar.Models;
using MZ.Util;
using Prism.Commands;
using Prism.Ioc;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using static MZ.Sidebar.MZEvents;

namespace MZ.Dashboard.ViewModels
{
    public class ImageDataTransmissionViewModel : MZBindableBase
    {
        #region Service
        private readonly IProducerService _producerService;
        #endregion

        #region Params
        public ObservableCollection<IconButtonModel> ActionButtons { get; } = [];
        public XrayDataProcesser XrayData => _producerService.XrayData;
        #endregion

        #region Command
        private DelegateCommand _loadCommand;
        public ICommand LoadCommand => _loadCommand ??= new(MZAction.Wrapper(LoadButton));

        private DelegateCommand _playpauseCommand;
        public ICommand PlayPauseCommand => _playpauseCommand ??= new(MZAction.Wrapper(PlayPauseButton));

        private DelegateCommand _stopCommand;
        public ICommand StopCommand => _stopCommand ??= new(MZAction.Wrapper(StopButton));
        #endregion

        public ImageDataTransmissionViewModel(IContainerExtension container, IProducerService producerService) : base(container)
        {
            _producerService = producerService;

            base.Initialize();
        }

        #region Initialize
        public override void InitializeModel()
        {
            ActionButtons.Add(new(nameof(PackIconMaterialKind.FileImport), LoadCommand));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Play), PlayPauseCommand));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Stop), StopCommand));
        }

        public override void InitializeEvent()
        {
            SubscribeEvent<Select, MenuModel>(async (MenuModel model) =>
            {
                await _producerService.LoadFilesAsync(model.Title);
            });
        }
        #endregion

        #region Button
        private async void LoadButton()
        {
            await _producerService.LoadAsync();
        }

        private void PlayPauseButton()
        {
            var pickerButton = ActionButtons.FirstOrDefault(vb => vb.Command == PlayPauseCommand);
            if (pickerButton != null)
            {
                pickerButton.IconKind = pickerButton.IconKind == nameof(PackIconMaterialKind.Play) ? nameof(PackIconMaterialKind.Pause) : nameof(PackIconMaterialKind.Play);
            }

            _producerService.Pause();
        }

        private void StopButton()
        {
            _producerService.Stop();
        }
        #endregion
    }
}