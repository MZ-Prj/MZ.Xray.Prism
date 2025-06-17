using MZ.Core;
using MZ.Toolbar.Views;
using Prism.Ioc;

namespace MZ.Toolbar
{
    public class ToolbarModule : MZModule
    {
        public ToolbarModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(MZRegionNames.ToolbarRegion, nameof(ToolbarView));
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ToolbarView>();
        }


        public override void InitializeEvent()
        {
        }

    }
}