using Prism.Ioc;
using MZ.Core;
using MZ.Dashboard.Views;
using static MZ.Core.MZEvent;
using static MZ.Core.MZModel;
using Prism.Events;

namespace MZ.Dashboard.ViewModels
{
    public class DashboardWindowViewModel : MZBindableBase
    {
        public DashboardWindowViewModel(IContainerExtension container) : base(container)
        {
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
