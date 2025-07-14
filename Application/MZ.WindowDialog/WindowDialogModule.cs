using MZ.Core;
using MZ.WindowDialog.Views;
using Prism.Ioc;

namespace MZ.WindowDialog
{
    public class WindowDialogModule : MZModule
    {
        public WindowDialogModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(MZRegionNames.WindowDialogRegion, nameof(WindowDialogView));
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WindowDialogView>();
        }

        public override void InitializeEvent()
        {
        }
    }
}