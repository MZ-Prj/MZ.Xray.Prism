using MZ.Core;
using MZ.Loading.Views;
using Prism.Ioc;
using Prism.Regions;

namespace MZ.Loading
{
    public class LoadingModule : MZModule
    {
        public LoadingModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(MZRegionNames.LoadingRegion, nameof(LoadingView));
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<LoadingView>();
        }

        public override void InitializeEvent()
        {
        }

    }
}