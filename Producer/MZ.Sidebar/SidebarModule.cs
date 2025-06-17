using MZ.Core;
using MZ.Sidebar.Views;
using Prism.Ioc;

namespace MZ.Sidebar
{
    public class SidebarModule : MZModule
    {
        public SidebarModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(MZRegionNames.SidebarRegion, nameof(SidebarView));
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<SidebarView>();
        }

        public override void InitializeEvent()
        {
        }
    }
}