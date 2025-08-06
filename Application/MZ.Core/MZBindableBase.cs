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
    /// <summary>
    /// Prism MVVM 프레임워크 기반에서 공통적으로 사용되는 ViewModel 베이스 클래스
    /// </summary>
    public abstract class MZBindableBase : BindableBase, INavigationAware, IDestructible, IDisposable
    {
        #region Dispatcher
        /// <summary>
        /// UI Thread 작업을 수행할 Dispatcher
        /// </summary>
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
        /// <summary>
        /// EventAggregator를 통한 이벤트 구독 목록 관리 
        /// </summary>
        private readonly List<(EventBase eventBase, SubscriptionToken eventToken)> _subscriptions = [];
        #endregion

        #region Services
        #endregion

        protected MZBindableBase(IContainerExtension container)
        {
            InitializeBaseServices(container);
        }

        /// <summary>
        /// DI 컨테이너로부터 기본 인프라 서비스(Provider) Resolve 및 초기화  
        /// </summary>
        protected void InitializeBaseServices(IContainerExtension container)
        {
            _eventAggregator = container.Resolve<IEventAggregator>();
            _regionManager = container.Resolve<IRegionManager>();
            _dialogService = container.Resolve<IDialogService>();
        }

        #region Initialize

        /// <summary>
        /// ViewModel 공통 초기화 진입점
        /// - 모델/이벤트/코어 등 세부 초기화 메서드 체이닝
        /// </summary>
        protected virtual void Initialize()
        {
            InitializeModel();
            InitializeEvent();
            InitializeCore();
        }

        /// <summary>
        /// 서비스 또는 DI 객체 별도 초기화 
        /// </summary>
        public virtual void InitializeServices(IContainerExtension container)
        {

        }

        /// <summary>
        /// 핵심 로직 별도 초기화ㅌ
        /// </summary>
        public virtual void InitializeCore()
        {
        }

        /// <summary>
        /// 모델/데이터 등 뷰모델의 상태 값 초기화
        /// </summary>
        public virtual void InitializeModel()
        {
        }
        /// <summary>
        /// 이벤트 구독 및 핸들러 등록
        /// </summary>
        public virtual void InitializeEvent()
        {
        }

        #endregion

        #region Event
        /// <summary>
        /// TODO : 검증 후 사용
        /// Prism EventAggregator 기반으로 PubSubEvent 구독 헬퍼
        /// </summary>
        protected void SubscribeEvent<TEvent, TPayload>(Action<TPayload> action, ThreadOption threadOption = ThreadOption.UIThread, bool keepSubscriberReferenceAlive = true) where TEvent : PubSubEvent<TPayload>, new()
        {
            var eventBase = _eventAggregator.GetEvent<TEvent>();
            var eventToken = eventBase.Subscribe(action, threadOption, keepSubscriberReferenceAlive);
            _subscriptions.Add((eventBase, eventToken));
        }
        #endregion

        #region Navigate
        /// <summary>
        /// NavigationContext로부터 View가 진입(Navigated)할 때 호출
        /// </summary>
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// TODO : 검증 후 사용
        /// 현재 ViewModel이 동일 Navigation Context에서 재사용 가능한지 결정
        /// </summary>
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// NavigationContext에서 View가 이탈할 때 호출
        /// </summary>
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion

        #region Clear
        /// <summary>
        /// ViewModel 파괴 시 이벤트 구독 해제
        /// </summary>
        public virtual void Destroy()
        {
            foreach (var (eventBase, eventToken) in _subscriptions)
            {
                eventBase.Unsubscribe(eventToken);
            }
            _subscriptions.Clear();
        }
        /// <summary>
        /// Dispose 패턴 구현(명시적 리소스 해제)
        /// </summary>
        public virtual void Dispose()
        {
            Destroy();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
