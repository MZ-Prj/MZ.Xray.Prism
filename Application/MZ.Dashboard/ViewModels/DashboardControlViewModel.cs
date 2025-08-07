using MZ.Core;
using MZ.Resource;
using Prism.Ioc;
using Prism.Regions;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// Dashboard Control ViewModel : 메인 화면을 보여주기 위한 중간 담당 계층
    /// </summary>
    public class DashboardControlViewModel : MZBindableBase
    {
        public DashboardControlViewModel(IContainerExtension container) : base(container)
        {
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

    }
}
