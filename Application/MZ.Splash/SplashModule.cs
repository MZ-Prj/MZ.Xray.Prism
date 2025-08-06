using MZ.Core;
using MZ.Splash.Views;
using Prism.Events;
using Prism.Ioc;
using static MZ.Event.MZEvent;

namespace MZ.Splash
{
    public class SplashModule : MZWindowModule
    {
        public SplashModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            SetRegion<SplashWindowView>(MZWindowNames.SplashWindow,(MZRegionNames.SplashRegion, nameof(SplashWindowView)));
            ShowWindow(MZWindowNames.SplashWindow);
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterViews(containerRegistry);
        }

        /// <summary>
        /// Splash 종료 이벤트 구독
        /// </summary>
        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<SplashCloseEvent>().Subscribe(() =>
            {
                CloseWindow(MZWindowNames.SplashWindow);
            }, ThreadOption.UIThread, true);
        }
    }
}