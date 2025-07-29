using MZ.Core;
using MZ.Dashboard.Views;
using Prism.Events;
using Prism.Ioc;
using static MZ.Event.MZEvent;

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
            _eventAggregator.GetEvent<SplashCloseEvent>().Subscribe(SplashCloseEvent, ThreadOption.UIThread, true);

            _eventAggregator.GetEvent<WindowOpenEvent>().Subscribe(WindowOpenEvent);
            _eventAggregator.GetEvent<WindowCloseEvent>().Subscribe(WindowCloseEvent);
            _eventAggregator.GetEvent<WindowHideEvent>().Subscribe(WindowHideEvent);
        }

        private void WindowOpenEvent(string model)
        {
            ShowWindow(model);
        }
        
        private void WindowCloseEvent(string model)
        {
            CloseWindow(model);
        }

        private void WindowHideEvent(string model)
        {
            HideWindow(model);
        }

        private void SplashCloseEvent()
        {
            SetWindowLocate(MZWindowNames.DashboardWindow, 1);
            ShowWindow(MZWindowNames.DashboardWindow);
        }
    }
}