using MZ.Core;
using MZ.Dashboard.Views;
using Prism.Events;
using Prism.Ioc;
using static MZ.Core.MZEvent;

namespace MZ.Dashboard
{
    public class DashboardModule : MZWindowModule
    {

        public DashboardModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            SetRegion<DashboardWindowView>(MZWindowNames.DashboardWindow, (MZRegionNames.DashboardRegion, nameof(DashboardControlView)));
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterViews(containerRegistry);
        }

        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<SplashCloseEvent>().Subscribe(() =>
            {
                SetWindowLocate(MZWindowNames.DashboardWindow, 0);
                ShowWindow(MZWindowNames.DashboardWindow);
            }, ThreadOption.UIThread, true);
        }
    }
}