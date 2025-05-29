using MZ.Blank.Views;
using MZ.Core;
using Prism.Ioc;

namespace MZ.Blank
{
    public class BlankModule : MZModule
    {

        public BlankModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(MZRegionNames.DialogRegion, nameof(BlankView));
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<BlankView>();
        }

        public override void InitializeEvent()
        {
        }
    }
}