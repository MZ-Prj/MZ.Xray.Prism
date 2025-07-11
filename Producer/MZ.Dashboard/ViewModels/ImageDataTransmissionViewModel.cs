using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Producer.Engine;
using MZ.Sidebar.Models;
using MZ.Domain.Models;
using MZ.Util;
using MZ.Loading;
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
        private readonly ILoadingService _loadingService;
        private readonly IProducerService _producerService;
        #endregion

        #region Params
        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.DashboardRegion]; set => SetProperty(ref _loadingModel, value); }

        private ObservableCollection<IconButtonModel> _actionButtons = [];
        public ObservableCollection<IconButtonModel> ActionButtons { get => _actionButtons; set => SetProperty(ref _actionButtons, value); }

        public XrayDataProcesser XrayData => _producerService.XrayData;
        public SocketProcesser Socket => _producerService.Socket;
        #endregion

        #region Command
        private DelegateCommand _loadCommand;
        public ICommand LoadCommand => _loadCommand ??= new(MZAction.Wrapper(LoadButton));

        private DelegateCommand _playpauseCommand;
        public ICommand PlayPauseCommand => _playpauseCommand ??= new(MZAction.Wrapper(PlayPauseButton));

        private DelegateCommand _stopCommand;
        public ICommand StopCommand => _stopCommand ??= new(MZAction.Wrapper(StopButton));
        #endregion

        public ImageDataTransmissionViewModel(IContainerExtension container, ILoadingService loadingService, IProducerService producerService) : base(container)
        {
            _producerService = producerService;
            _loadingService = loadingService;

            base.Initialize();
        }

        #region Initialize
        public override void InitializeModel()
        {
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Play), PlayPauseCommand, isVisibility:false));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Stop), StopCommand, isVisibility: false));
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
            // logic
            using (_loadingService[MZRegionNames.DashboardRegion].Show())
            {
                await _producerService.LoadAsync();
            }

            // ui
            foreach (var button in ActionButtons)
            {
                button.IsVisibility = Socket.Model.IsConnected;
            }

            // logic 
            if (Socket.Model.IsConnected)
            {
                await _producerService.RunAsync();
            }
            else
            {
                _producerService.Stop();
            }

        }

        private void PlayPauseButton()
        {
            //ui
            var button = ActionButtons.FirstOrDefault(vb => vb.Command == PlayPauseCommand);
            if (button != null)
            {
                button.IconKind = button.IconKind == nameof(PackIconMaterialKind.Play) ? nameof(PackIconMaterialKind.Pause) : nameof(PackIconMaterialKind.Play);
            }

            //logic
            _producerService.Pause();

        }

        private void StopButton()
        {
            // ui
            foreach (var actionButton in ActionButtons)
            {
                actionButton.IsVisibility = false;
            }

            var button = ActionButtons.FirstOrDefault(vb => vb.Command == PlayPauseCommand);
            if (button != null)
            {
                button.IconKind = nameof(PackIconMaterialKind.Play);
            }

            // logic
            _producerService.Stop();

        }
        #endregion
    }
}