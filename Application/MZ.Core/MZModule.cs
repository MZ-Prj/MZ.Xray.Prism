using Prism.Ioc;
using Prism.Modularity;

namespace MZ.Core
{
    /// <summary>
    /// Prism 모듈 베이스 클래스
    /// - Prism의 IModule을 상속하여 모듈화 구조 지원
    /// - 모듈 초기화, 타입 등록, 이벤트 초기화의 표준화된 진입점을 정의
    /// - 실제 모듈 구현체는 이 클래스를 상속받아 RegisterTypes, OnInitialized, InitializeEvent를 필수 구현
    /// </summary>
    public abstract class MZModule : MZModuleBase, IModule
    {
        protected MZModule(IContainerExtension container) : base(container)
        {
        }

        /// <summary>
        /// 모듈이 애플리케이션에 로드된 후 한 번 호출  
        /// - 의존 객체는 IContainerProvider를 통해 Resolve
        /// </summary>
        public abstract void OnInitialized(IContainerProvider containerProvider);
        /// <summary>
        /// DI 컨테이너에 타입(서비스, 뷰모델, 뷰 등) 등록  
        /// - 모듈 내에서 사용할 객체/서비스를 미리 등록하는 역할
        /// - 싱글턴/트랜지언트 등 라이프사이클 지정 가능
        /// </summary>
        public abstract void RegisterTypes(IContainerRegistry containerRegistry);
        /// <summary>
        /// 이벤트 구독 및 핸들러 초기화용 메서드  
        /// - EventAggregator 기반의 이벤트 바인딩, 메시지/알림 
        /// </summary>
        public abstract void InitializeEvent();
    }
}
