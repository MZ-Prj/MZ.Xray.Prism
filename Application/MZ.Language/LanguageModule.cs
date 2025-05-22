using MZ.Core;
using MZ.Language.Views;
using Prism.Ioc;

namespace MZ.Language
{
    public class LanguageModule : MZModule
    {
        public LanguageModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(MZRegionNames.LanguageRegion, nameof(LanguageView));
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LanguageView>();
        }

        public override void InitializeEvent()
        {
        }
    }
}