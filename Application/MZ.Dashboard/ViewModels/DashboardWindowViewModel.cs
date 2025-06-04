using MZ.Core;
using MZ.Dashboard.Models;
using MZ.Loading;
using MZ.Util;
using MZ.Resource;
using MZ.Infrastructure;
using MZ.Auth.Views;
using MahApps.Metro.IconPacks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Prism.Ioc;
using Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;
using static MZ.Core.MZEvent;
using static MZ.Core.MZModel;
using MZ.Domain.Models;

namespace MZ.Dashboard.ViewModels
{
    public class DashboardWindowViewModel : MZBindableBase
    {
        #region Services
        private readonly LoadingService _loadingService;
        #endregion

        #region Models
        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.DashboardRegion]; set => SetProperty(ref _loadingModel, value); }

        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; } = [];
        #endregion

        #region Commands
        private DelegateCommand _themeCommand;
        public ICommand ThemeCommand => _themeCommand ??= new(MZAction.Wrapper(ThemeButton));

        private DelegateCommand _languageCommand;
        public ICommand LanguageCommand => _languageCommand ??= new(MZAction.Wrapper(LanguageButton));

        private DelegateCommand _logoutCommand;
        public ICommand LogoutCommand => _logoutCommand ??= new DelegateCommand(MZAction.Wrapper(LogoutButton));
        #endregion

        private readonly DatabaseService _databaseService;
        public DashboardWindowViewModel(IContainerExtension container) : base(container)
        {
            _databaseService = container.Resolve<DatabaseService>();
            _loadingService = container.Resolve<LoadingService>();
        }

        public override void InitializeModel()
        {
            
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
    }
}
