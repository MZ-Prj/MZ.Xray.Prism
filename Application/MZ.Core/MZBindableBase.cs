using System.Windows.Threading;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace MZ.Core
{
    public abstract class MZBindableBase : BindableBase, INavigationAware
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
        /// 모듈 로드 및 관리
        /// 동적 모듈 로드 및 해제 기능을 제공
        /// </summary>
        protected IModuleManager _moduleManager;

        /// <summary>
        /// DialogService는 대화 상자를 관리하고 표시하는 데 사용
        /// 사용자와의 상호작용을 위한 팝업 대화 상자 기능을 제공
        /// </summary>
        protected IDialogService _dialogService;
        #endregion

        #region Services
        #endregion

        protected MZBindableBase(IContainerExtension container)
        {
            InitializeServices(container);
            InitializeCore();
            InitializeModel();
            InitializeEvent();
        }

        protected void InitializeServices(IContainerExtension container)
        {
            _eventAggregator = container.Resolve<IEventAggregator>();
            _regionManager = container.Resolve<IRegionManager>();
            _moduleManager = container.Resolve<IModuleManager>();
            _dialogService = container.Resolve<IDialogService>();
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
    }
}
