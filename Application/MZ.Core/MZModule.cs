using Prism.Ioc;
using Prism.Modularity;

namespace MZ.Core
{
    public abstract class MZModule : MZModuleBase, IModule
    {
        protected MZModule(IContainerExtension container) : base(container)
        {
        }

        public abstract void OnInitialized(IContainerProvider containerProvider);
        public abstract void RegisterTypes(IContainerRegistry containerRegistry);
        public abstract void InitializeEvent();
    }
}
