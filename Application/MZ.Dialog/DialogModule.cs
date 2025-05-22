using MZ.Core;
using MZ.Dialog.Views;
using Prism.Ioc;

namespace MZ.Dialog
{
    public class DialogModule : MZModule
    {
        public DialogModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(MZRegionNames.DialogRegion, nameof(DialogView));
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DialogView>();
        }

        public override void InitializeEvent()
        {
        }
    }
}