using MZ.Core;
using Prism.Ioc;
using Prism.Regions;
using System;

namespace MZ.Dashboard.ViewModels
{
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
