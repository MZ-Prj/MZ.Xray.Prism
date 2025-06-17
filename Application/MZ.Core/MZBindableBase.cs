using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace MZ.Core
{
    public abstract class MZBindableBase : BindableBase, INavigationAware, IDestructible, IDisposable
    {
        #region Dispatcher
        protected readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        #endregion

        #region Provider
        /// <summary>
        /// 이벤트를 전달하는 Event Aggregator
        /// 모듈 간 통신이나 이벤트 전달에 사용
        /// </summary>
        protected IEventAggregator _eventAggregator;

        /// <summary>
        /// UI에서 영역(Region)을 관리하고 탐색을 처리하는 데 사용
        /// </summary>
        protected IRegionManager _regionManager;

        /// <summary>
        /// DialogService는 대화 상자를 관리하고 표시하는 데 사용
        /// 사용자와의 상호작용을 위한 팝업 대화 상자 기능을 제공
        /// </summary>
        protected IDialogService _dialogService;
        #endregion



        #region Subscription Management
        private readonly List<(EventBase eventBase, SubscriptionToken eventToken)> _subscriptions = [];
        #endregion

        #region Services
        #endregion

        protected MZBindableBase(IContainerExtension container)
        {
            InitializeBaseServices(container);
        }

        protected void InitializeBaseServices(IContainerExtension container)
        {
            _eventAggregator = container.Resolve<IEventAggregator>();
            _regionManager = container.Resolve<IRegionManager>();
            _dialogService = container.Resolve<IDialogService>();
        }

        #region Initialize

        protected virtual void Initialize()
        {
            InitializeModel();
            InitializeEvent();
            InitializeCore();
        }

        public virtual void InitializeServices(IContainerExtension container)
        {

        }

        public virtual void InitializeCore()
        {
        }

        public virtual void InitializeModel()
        {
        }

        public virtual void InitializeEvent()
        {
        }

        #endregion

        #region Event
        protected void SubscribeEvent<TEvent, TPayload>(Action<TPayload> action, ThreadOption threadOption = ThreadOption.UIThread, bool keepSubscriberReferenceAlive = true) where TEvent : PubSubEvent<TPayload>, new()
        {
            var eventBase = _eventAggregator.GetEvent<TEvent>();
            var eventToken = eventBase.Subscribe(action, threadOption, keepSubscriberReferenceAlive);
            _subscriptions.Add((eventBase, eventToken));
        }
        #endregion


        #region Navigate
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion

        #region Clear
        public virtual void Destroy()
        {
            foreach (var (eventBase, eventToken) in _subscriptions)
            {
                eventBase.Unsubscribe(eventToken);
            }
            _subscriptions.Clear();
        }

        public virtual void Dispose()
        {
            Destroy();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
