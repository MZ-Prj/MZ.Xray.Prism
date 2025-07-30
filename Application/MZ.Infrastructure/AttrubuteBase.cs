using System;

namespace MZ.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RepositoryAttribute : Attribute { }
}
