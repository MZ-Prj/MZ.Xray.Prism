using System;
using System.Linq;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;
using MZ.Logger;

namespace MZ.Core
{
    public abstract class MZModuleBase
    {
        #region 
        private static readonly Lazy<Type[]> _defaultViewTypes = new(() =>
                        [.. typeof(MZModuleBase).Assembly
                            .GetExportedTypes()
                            .Where(IsValidViewType)]);
        private static bool IsValidViewType(Type type)
        {
            return typeof(FrameworkElement).IsAssignableFrom(type)
                   && !type.IsAbstract
                   && !type.ContainsGenericParameters;
        }
        #endregion

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
        /// DI ContainerExtension을 처리하는데 사용
        /// </summary>
        protected IContainerExtension _container;

        /// <summary>
        /// 주어진 DI 컨테이너에서 필요한 Prism 모듈 초기화 작업 실행
        /// </summary>
        /// <param name="container">필요한 모듈 제공</param>
        protected MZModuleBase(IContainerExtension container)
        {
            InitializeServices(container);
        }

        /// <summary>
        /// Prism에서 제공하는 주요 서비스를 초기화
        /// EventAggregator, RegionManager, ModuleManager, DialogService를 DI 컨테이너로부터 할당 받음
        /// </summary>
        /// <param name="container">서비스 생성 및 제공하는 역할</param>
        protected void InitializeServices(IContainerExtension container)
        {
            _container = container;
            _eventAggregator = container.Resolve<IEventAggregator>();
            _regionManager = container.Resolve<IRegionManager>();
        }

        /// <summary>
        /// 특정 Region에 Navigation을 수행
        /// Region이 등록되지 않았을 경우, 등록 후 Navigation을 수행하도록 처리
        /// </summary>
        /// <param name="regionName">대상 Region의 이름</param>
        /// <param name="viewName">탐색할 View의 이름</param>
        protected virtual void NavigateToRegion(string regionName, string viewName)
        {
            if (_regionManager.Regions.ContainsRegionWithName(regionName))
            {
                _regionManager.RequestNavigate(regionName, viewName);
            }
            else
            {
                _regionManager.Regions.CollectionChanged += (s, e) =>
                {
                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add &&
                        _regionManager.Regions.ContainsRegionWithName(regionName))
                    {
                        _regionManager.RequestNavigate(regionName, viewName);
                    }
                };
            }
        }

        /// <summary>
        /// Window에 대한 RegionManager 초기화
        /// </summary>
        /// <param name="window"></param>
        protected virtual void InitializeRegion(Window window)
        {
            try
            {
                var regionManager = _container.Resolve<IRegionManager>();
                RegionManager.SetRegionManager(window, regionManager);
                RegionManager.UpdateRegions();
            }
            catch (Exception ex)
            {
                MZLogger.Warning(ex.ToString());
            }
        }

        /// <summary>
        /// 현재 클래스가 속한 어셈블리 내의 모든 public하고 
        /// 추상 클래스가 아닌 FrameworkElement를 상속받은 뷰를 등록 
        /// 뷰 이름이 "View"로 끝나면 해당 뷰를 네이밍 규칙에 맞는 Region에 등록
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected virtual void RegisterViews(IContainerRegistry containerRegistry)
        {
            const string viewSuffix = "View";
            const string regionSuffix = "Region";

            foreach (var view in _defaultViewTypes.Value)
            {
                containerRegistry.RegisterForNavigation(view, view.Name);

                if (view.Name.EndsWith(viewSuffix))
                {
                    var regionName = string.Concat(view.Name.AsSpan(0, view.Name.Length - viewSuffix.Length), regionSuffix);
                    _regionManager.RegisterViewWithRegion(regionName, view);
                }
            }
        }

    }
}
