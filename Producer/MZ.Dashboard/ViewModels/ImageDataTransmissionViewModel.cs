using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Producer.Engine;
using MZ.Sidebar.Models;
using MZ.Util;
using MZ.Model;
using MZ.Loading;
using Prism.Commands;
using Prism.Ioc;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static MZ.Sidebar.MZEvents;

namespace MZ.Dashboard.ViewModels
{
    public class ImageDataTransmissionViewModel : MZBindableBase
    {
        #region Service
        private readonly ILoadingService _loadingService;
        private readonly IProducerService _producerService;
        private readonly IConfiguration _configuration;
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

        private DelegateCommand _fileDownloadCommand;
        public ICommand FileDownloadCommand => _fileDownloadCommand ??= new(MZAction.Wrapper(FileDownloadButton));

        private DelegateCommand _loadCommand;
        public ICommand LoadCommand => _loadCommand ??= new(MZAction.Wrapper(LoadButton));


        private DelegateCommand _playpauseCommand;
        public ICommand PlayPauseCommand => _playpauseCommand ??= new(MZAction.Wrapper(PlayPauseButton));

        private DelegateCommand _stopCommand;
        public ICommand StopCommand => _stopCommand ??= new(MZAction.Wrapper(StopButton));
        #endregion

        public ImageDataTransmissionViewModel(IContainerExtension container, ILoadingService loadingService, IProducerService producerService, IConfiguration configuration) : base(container)
        {
            _producerService = producerService;
            _loadingService = loadingService;
            _configuration = configuration;
            base.Initialize();
        }

        #region Initialize
        public override void InitializeModel()
        {

            ActionButtons.Add(new(nameof(PackIconMaterialKind.Download), FileDownloadCommand));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Lan), LoadCommand, isVisibility: true));
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
        private async void FileDownloadButton()
        {
            MZWebDownload download = new();

            string path = _configuration["Data:Path"];
            string link = _configuration["Data:DownloadLink"];

            MZIO.TryDeleteFile(path);
            MZIO.TryMakeDirectoryRemoveFile(path);

            using (LoadingModel.Show())
            {
                LoadingModel.Message = "Downloading...";
                bool checkDownload = await download.RunAsync(link, path);

                if (checkDownload)
                {
                    string extractPath = Path.GetDirectoryName(path);

                    LoadingModel.Message = "Unzip Dataset...";
                    bool isUnzip = await MZZip.UnzipAsync(path, extractPath);
                    if (isUnzip)
                    {
                        LoadingModel.Message = "Success!";
                        MZIO.TryDeleteFile(path);

                        LoadingModel.Message = "Read Dataset...";
                        _producerService.Stop();
                        await _producerService.LoadFilesAsync(extractPath);
                    }
                    else
                    {
                        LoadingModel.Message = "Fail!";
                    }
                    await Task.Delay(1000);
                }
            }
        }

        private async void LoadButton()
        {
            // logic
            using (LoadingModel.Show())
            {
                await _producerService.LoadAsync();

                VisibilityButton(FileDownloadCommand, !Socket.Model.IsConnected, ActionButtons);
                VisibilityButton(LoadCommand, !Socket.Model.IsConnected, ActionButtons);
                VisibilityButton(PlayPauseCommand, Socket.Model.IsConnected, ActionButtons);
                VisibilityButton(StopCommand, Socket.Model.IsConnected, ActionButtons);

                SetButtonIcon(PlayPauseCommand, nameof(PackIconMaterialKind.Play), ActionButtons);
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
            ToggleButton(PlayPauseCommand, nameof(PackIconMaterialKind.Play), nameof(PackIconMaterialKind.Pause), ActionButtons);

            VisibilityButton(LoadCommand, false, ActionButtons);

            //logic
            _producerService.Pause();

        }

        private void StopButton()
        {
            SetButtonIcon(PlayPauseCommand, nameof(PackIconMaterialKind.Play), ActionButtons);
            
            VisibilityButton(FileDownloadCommand, true, ActionButtons);
            VisibilityButton(LoadCommand, true, ActionButtons);
            VisibilityButton(PlayPauseCommand, false, ActionButtons);
            VisibilityButton(StopCommand, false, ActionButtons);

            // logic
            _producerService.Stop();

        }
        #endregion


        /// <summary>
        /// 버튼 아이콘 On/Off 토글
        /// </summary>
        /// <param name="targetCommand">ICommand</param>
        /// <param name="iconOn">string</param>
        /// <param name="iconOff">string</param>
        /// <param name="buttonCollections">ObservableCollection<IconButtonModel>[]</param>
        private void ToggleButton(ICommand targetCommand, string iconOn, string iconOff, params ObservableCollection<IconButtonModel>[] buttonCollections)
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

        private void SetButtonIcon(ICommand command, string iconKind, ObservableCollection<IconButtonModel> actionButtons)
        {
            var button = actionButtons?.FirstOrDefault(vb => vb.Command == command);
            if (button != null)
            {
                button.IconKind = iconKind;
            }
        }


        /// <summary>
        /// 버튼 노출상태 변경
        /// </summary>
        /// <param name="targetCommand">ICommand</param>
        /// <param name="isVisibility">bool</param>
        /// <param name="buttonCollections">ObservableCollection<IconButtonModel>[]</param>
        private void VisibilityButton(ICommand targetCommand, bool isVisibility, params ObservableCollection<IconButtonModel>[] buttonCollections)
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
    }
}