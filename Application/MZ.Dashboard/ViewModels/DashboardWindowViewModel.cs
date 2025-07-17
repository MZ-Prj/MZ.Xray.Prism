using MZ.Core;
using MZ.Loading;
using MZ.Util;
using MZ.Resource;
using MZ.Infrastructure;
using MZ.Auth.Views;
using MZ.Domain.Models;
using MZ.WindowDialog;
using MZ.Dashboard.Views;
using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Prism.Ioc;
using Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;
using static MZ.Core.MZModel;
using static MZ.Event.MZEvent;

namespace MZ.Dashboard.ViewModels
{
    public class DashboardWindowViewModel : MZBindableBase
    {
        #region Services
        private readonly ILoadingService _loadingService;
        private readonly IDatabaseService _databaseService;
        private readonly IWindowDialogService _windowDialogService;
        #endregion

        #region Models
        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.DashboardRegion]; set => SetProperty(ref _loadingModel, value); }

        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; } = [];
        #endregion

        #region Commands

        private DelegateCommand _imageStorageCommand;
        public ICommand ImageStorageCommand => _imageStorageCommand ??= new DelegateCommand(MZAction.Wrapper(ImageStorageButton));

        private DelegateCommand _logStorageCommand;
        public ICommand LogStorageCommand => _logStorageCommand ??= new DelegateCommand(MZAction.Wrapper(LogStorageButton));

        private DelegateCommand _recordCommand;
        public ICommand RecordCommand => _recordCommand ??= new DelegateCommand(MZAction.Wrapper(RecordButton));

        private DelegateCommand _materialCommand;
        public ICommand MaterialCommand => _materialCommand ??= new DelegateCommand(MZAction.Wrapper(MaterialButton));

        private DelegateCommand aiCommand;
        public ICommand AICommand => aiCommand ??= new DelegateCommand(MZAction.Wrapper(AIButton));

        private DelegateCommand windowClosingCommand;
        public ICommand WindowClosingCommand => windowClosingCommand ??= new DelegateCommand(WindowClosing);

        private DelegateCommand _themeCommand;
        public ICommand ThemeCommand => _themeCommand ??= new(MZAction.Wrapper(ThemeButton));

        private DelegateCommand _languageCommand;
        public ICommand LanguageCommand => _languageCommand ??= new(MZAction.Wrapper(LanguageButton));

        private DelegateCommand _logoutCommand;
        public ICommand LogoutCommand => _logoutCommand ??= new DelegateCommand(MZAction.Wrapper(LogoutButton));

        #endregion


        public DashboardWindowViewModel(IContainerExtension container, IDatabaseService databaseService, ILoadingService loadingService, IWindowDialogService windowDialogService) : base(container)
        {
            _databaseService = databaseService;
            _loadingService = loadingService;
            _windowDialogService = windowDialogService;

            base.Initialize();

        }

        public override void InitializeModel()
        {
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.FormatColorFill), MaterialCommand, isVisibility: false));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Robot), aiCommand, isVisibility: false));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.FilePdfBox), RecordCommand, isVisibility: false));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.ImageSearch), ImageStorageCommand, isVisibility: false));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.FileSearch), LogStorageCommand, isVisibility: false));

            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Earth), LanguageCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.ThemeLightDark), ThemeCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Logout), LogoutCommand, isVisibility: false));
        }

        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<DashboardNavigationEvent>().Subscribe((NavigationModel model) =>
            {
                _regionManager.RequestNavigate(model.Region, model.View);
                UpdateWindowCommandButton(model.View == MZViewNames.DashboardControlView);
            }, ThreadOption.UIThread, true);
        }

        private void WindowClosing()
        {
            Application.Current.Shutdown();
        }

        private void ThemeButton()
        {
            _databaseService.User.ChangeTheme(new(ThemeService.ChangeMode()));
        }

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
                    _databaseService.User.ChangeLanguage(new(LanguageService.GetCurrentLanguageRole()));
                });
        }

        private void LogoutButton()
        {
            _databaseService.User.Logout();
            _eventAggregator.GetEvent<DashboardNavigationEvent>().Publish(
                        new NavigationModel(
                            MZRegionNames.DashboardRegion,
                            nameof(UserLoginView)));
        }

        private async void ImageStorageButton()
        {
            await _windowDialogService.ShowWindow(
                title: MZRegionNames.ImageStorageControl,
                regionName: nameof(ImageStorageControlView),
                isMultiple: false);
        }

        private async void LogStorageButton()
        {
            await _windowDialogService.ShowWindow(
                title: MZRegionNames.LogStorageControl,
                regionName: nameof(LogStorageControlView),
                isMultiple:false);
        }

        private void RecordButton()
        {
        }

        private void MaterialButton()
        {
        }

        private void AIButton()
        {
        }


        private void UpdateWindowCommandButton(bool check)
        {
            foreach (var button in WindowCommandButtons)
            {
                button.IsVisibility = check ||
                                      button.IconKind == nameof(PackIconMaterialKind.Earth) ||
                                      button.IconKind == nameof(PackIconMaterialKind.ThemeLightDark);
            }
        }
    }
}
