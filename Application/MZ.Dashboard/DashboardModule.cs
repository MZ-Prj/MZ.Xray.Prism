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
            SetRegion<DashboardWindowView>(MZWindowNames.DashboardWindow, (MZRegionNames.DashboardRegion, nameof(DashboardWindowView)));
            SetRegion<AnalysisWindowView>(MZWindowNames.AnalysisWindow, (MZRegionNames.AnalysisRegion, nameof(AnalysisWindowView)));
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
                SetWindowLocate(MZWindowNames.AnalysisWindow, 1);

                ShowWindow(MZWindowNames.DashboardWindow);
                ShowWindow(MZWindowNames.AnalysisWindow);

            }, ThreadOption.UIThread, true);
        }
    }
}