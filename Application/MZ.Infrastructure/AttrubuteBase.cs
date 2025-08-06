using System;

namespace MZ.Infrastructure
{
    /// <summary>
    /// 서비스(Service) 클래스에 부여하는 커스텀 특성(Attribute)
    /// <para>- 의존성 주입(DI) 및 서비스 자동 등록 등의 목적으로 사용</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceAttribute : Attribute { }

    /// <summary>
    /// 리포지토리(Repository) 클래스에 부여하는 커스텀 특성(Attribute)
    /// <para>- 의존성 주입(DI) 및 리포지토리 자동 등록 등의 목적으로 사용</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RepositoryAttribute : Attribute { }
}
