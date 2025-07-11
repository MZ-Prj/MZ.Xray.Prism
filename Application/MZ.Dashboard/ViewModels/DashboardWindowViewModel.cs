using MZ.Core;
using MZ.Loading;
using MZ.Util;
using MZ.Resource;
using MZ.Infrastructure;
using MZ.Auth.Views;
using MZ.Domain.Models;
using MahApps.Metro.IconPacks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Prism.Ioc;
using Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;
using static MZ.Core.MZModel;
using static MZ.Event.MZEvent;
using System;
using MZ.Blank.Views;
using MZ.Dashboard.Views;
using System.Windows;

namespace MZ.Dashboard.ViewModels
{
    public class DashboardWindowViewModel : MZBindableBase
    {
        #region Services
        private readonly ILoadingService _loadingService;
        private readonly IDatabaseService _databaseService;
        #endregion

        #region Models
        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.DashboardRegion]; set => SetProperty(ref _loadingModel, value); }

        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; } = [];
        #endregion

        #region Commands

        private DelegateCommand windowClosingCommand;
        public ICommand WindowClosingCommand => windowClosingCommand ??= new DelegateCommand(WindowClosing);

        private DelegateCommand _themeCommand;
        public ICommand ThemeCommand => _themeCommand ??= new(MZAction.Wrapper(ThemeButton));

        private DelegateCommand _languageCommand;
        public ICommand LanguageCommand => _languageCommand ??= new(MZAction.Wrapper(LanguageButton));

        private DelegateCommand _logoutCommand;
        public ICommand LogoutCommand => _logoutCommand ??= new DelegateCommand(MZAction.Wrapper(LogoutButton));

        private DelegateCommand _analysisCommand;
        public ICommand AnalysisCommand => _analysisCommand ??= new DelegateCommand(MZAction.Wrapper(AnalysisButton));

        #endregion


        public DashboardWindowViewModel(IContainerExtension container, IDatabaseService databaseService, ILoadingService loadingService) : base(container)
        {
            _databaseService = databaseService;
            _loadingService = loadingService;

            base.Initialize();

        }

        public override void InitializeModel()
        {
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Graph), AnalysisCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Earth), LanguageCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.ThemeLightDark), ThemeCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Logout), LogoutCommand));
        }

        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<DashboardNavigationEvent>().Subscribe((NavigationModel model) =>
            {
                _regionManager.RequestNavigate(model.Region, model.View);

            }, ThreadOption.UIThread, true);
        }

        private void WindowClosing()
        {
            _eventAggregator.GetEvent<WindowCloseEvent>().Publish(MZWindowNames.AnalysisWindow);
            _eventAggregator.GetEvent<WindowCloseEvent>().Publish(MZWindowNames.DashboardWindow);

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
            _regionManager.RequestNavigate(MZRegionNames.DashboardRegion, nameof(UserLoginView));
        }

        private void AnalysisButton()
        {
            _eventAggregator.GetEvent<WindowOpenEvent>().Publish(MZWindowNames.AnalysisWindow);

        }

    }
}
