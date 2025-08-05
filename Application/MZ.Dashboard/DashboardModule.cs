using MZ.Core;
using MZ.Dashboard.Views;
using Prism.Events;
using Prism.Ioc;
using static MZ.Event.MZEvent;

namespace MZ.Dashboard
{
    /// <summary>
    /// Dashboard Module : Dashboard 모듈 등록 및 초기화 
    /// </summary>
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

        /// <summary>
        /// 윈도우 오픈 이벤트
        /// </summary>
        /// <param name="model">string</param>
        private void WindowOpenEvent(string model)
        {
            ShowWindow(model);
        }

        /// <summary>
        /// 윈도우 종료 이벤트 
        /// </summary>
        /// <param name="model">string</param>
        private void WindowCloseEvent(string model)
        {
            CloseWindow(model);
        }

        /// <summary>
        /// 윈도우 숨김 이벤트
        /// </summary>
        /// <param name="model">string</param>
        private void WindowHideEvent(string model)
        {
            HideWindow(model);
        }

        /// <summary>
        /// Splash 종료시 DashboardWindow 위치 지정 및 노출
        /// </summary>
        private void SplashCloseEvent()
        {
            SetWindowLocate(MZWindowNames.DashboardWindow, 1);
            ShowWindow(MZWindowNames.DashboardWindow);
        }
    }
}