using Microsoft.Win32;
using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Dashboard.Models;
using MZ.Domain.Models;
using MZ.Loading;
using MZ.Resource;
using MZ.Util;
using MZ.Sidebar.Models;
using MZ.Producer.Engine;
using Prism.Commands;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using static MZ.Core.MZEvent;
using static MZ.Core.MZModel;
using static MZ.Sidebar.MZEvents;
using System;

namespace MZ.Dashboard.ViewModels
{
    public class DashboardWindowViewModel : MZBindableBase
    {
        #region Services
        private readonly ILoadingService _loadingService;
        private readonly IProducerService _producerService;
        #endregion

        #region Params
        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.DashboardRegion]; set => SetProperty(ref _loadingModel, value); }

        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; } = [];
        #endregion

        #region Commands
        private DelegateCommand windowClosingCommand;
        public ICommand WindowClosingCommand => windowClosingCommand ??= new DelegateCommand(MZAction.Wrapper(WindowClosingButton));

        private DelegateCommand _ipNetworkCommand;
        public ICommand IpNetworkCommand => _ipNetworkCommand ??= new(MZAction.Wrapper(IpNetworkButton));
        
        private DelegateCommand _folderOpenCommand;
        public ICommand FolderOpenCommand => _folderOpenCommand ??= new(MZAction.Wrapper(FolderOpenButton));
        
        private DelegateCommand _themeCommand;
        public ICommand ThemeCommand => _themeCommand ??= new(MZAction.Wrapper(ThemeButton));

        private DelegateCommand _languageCommand;
        public ICommand LanguageCommand => _languageCommand ??= new(MZAction.Wrapper(LanguageButton));
        #endregion

        public DashboardWindowViewModel(IContainerExtension container, ILoadingService loadingService, IProducerService producerService) : base(container)
        {
            _loadingService = loadingService;
            _producerService = producerService;

            base.Initialize();
        }

        #region Initialize
        public override void InitializeModel()
        {
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.FolderOpen), FolderOpenCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.IpNetwork), IpNetworkCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Earth), LanguageCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.ThemeLightDark), ThemeCommand));
        }
        

        public override void InitializeEvent()
        {
            SubscribeEvent<DashboardNavigationEvent, NavigationModel>(model => _regionManager.RequestNavigate(model.Region, model.View));
        }
        #endregion

        #region Button
        /// <summary>
        /// Window Closing
        /// </summary>
        private void WindowClosingButton()
        {
            _producerService.Stop();
            Application.Current.Shutdown(); 
        }

        /// <summary>
        /// Theme (Dark/Light)
        /// </summary>
        private void ThemeButton()
        {
            ThemeService.ChangeMode();
        }

        /// <summary>
        /// Open Language Dialog
        /// </summary>
        private void LanguageButton()
        {
            _dialogService.ShowDialog(
                "DialogView",
                new DialogParameters
                {
                    {"Title",  MZRegionNames.LanguageRegion},
                    {"RegionName", MZRegionNames.LanguageRegion}
                },
                (IDialogResult result) => {
                });
        }

        /// <summary>
        /// Select Images Folder
        /// </summary>
        private async void FolderOpenButton()
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                ValidateNames = false,
                FileName = "Select Folder"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                using (_loadingService[MZRegionNames.DashboardRegion].Show())
                {
                    string directory = Path.GetDirectoryName(dialog.FileName);
                    _producerService.Stop();
                    await _producerService.LoadFilesAsync(directory);
                    _eventAggregator.GetEvent<Add>().Publish(new MenuModel() { IconKind = nameof(PackIconMaterialKind.Image), Title = directory});
                }
            }
        }

        /// <summary>
        /// Check Ip/Port
        /// </summary>
        private void IpNetworkButton()
        {
            _dialogService.ShowDialog(
                "DialogView",
                new DialogParameters
                {
                    {"Title",  MZRegionNames.IpNetworkRegion},
                    {"RegionName", MZRegionNames.IpNetworkRegion}
                },
                (IDialogResult result) => {
                });
        }
        #endregion
    }
}
