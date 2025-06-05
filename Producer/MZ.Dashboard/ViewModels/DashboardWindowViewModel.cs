using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Dashboard.Models;
using MZ.Domain.Models;
using MZ.Loading;
using MZ.Resource;
using MZ.Util;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Windows.Input;
using System.Collections.ObjectModel;
using static MZ.Core.MZEvent;
using static MZ.Core.MZModel;

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

        #endregion

        public DashboardWindowViewModel(IContainerExtension container) : base(container)
        {
            _loadingService = container.Resolve<LoadingService>();
        }

        public override void InitializeModel()
        {

            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.Earth), LanguageCommand));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.ThemeLightDark), ThemeCommand));
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
            ThemeService.ChangeMode();
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
                });
        }
    }
}
