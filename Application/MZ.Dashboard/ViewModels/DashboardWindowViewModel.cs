using Prism.Ioc;
using MZ.Core;
using static MZ.Core.MZEvent;
using static MZ.Core.MZModel;

namespace MZ.Dashboard.ViewModels
{
    public class DashboardWindowViewModel : MZBindableBase
    {
        public DashboardWindowViewModel(IContainerExtension container) : base(container)
        {
        }

        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<NavigationEvent>().Subscribe((NavigationModel model) =>
            {
                _regionManager.RequestNavigate(model.Region, model.View);
            });
        }
    }
}
