using MZ.Core;
using Prism.Events;
using Prism.Ioc;
using static MZ.Core.MZEvent;
using static MZ.Core.MZModel;

namespace MZ.Dashboard.ViewModels
{
    public class AnalysisWindowViewModel : MZBindableBase
    {
        public AnalysisWindowViewModel(IContainerExtension container) : base(container)
        {
            base.Initialize();
        }

        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<AnalysisNavigationEvent>().Subscribe((NavigationModel model) =>
            {
                _regionManager.RequestNavigate(model.Region, model.View);
            }, ThreadOption.UIThread, true);
        }
    }
}
