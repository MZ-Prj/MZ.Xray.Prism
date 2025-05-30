using MZ.Core;
using MZ.Dashboard.Models;
using Prism.Ioc;
using Prism.Events;
using System.Collections.ObjectModel;
using MZ.Loading;
using static MZ.Core.MZEvent;
using static MZ.Core.MZModel;
using MZ.Loading.Models;
using System.Threading.Tasks;
using System.Threading;

namespace MZ.Dashboard.ViewModels
{
    public class DashboardWindowViewModel : MZBindableBase
    {
        #region Services
        private readonly LoadingService _loadingService;
        #endregion

        #region Models
        public LoadingModel LoadingModel { get; set; }
        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; set; }
        #endregion

        public DashboardWindowViewModel(IContainerExtension container) : base(container)
        {
            _loadingService = container.Resolve<LoadingService>();
            LoadingModel = _loadingService[MZRegionNames.DashboardRegion];
        }

        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<DashboardNavigationEvent>().Subscribe((NavigationModel model) =>
            {
                _regionManager.RequestNavigate(model.Region, model.View);

            }, ThreadOption.UIThread, true);
        }
    }
}
