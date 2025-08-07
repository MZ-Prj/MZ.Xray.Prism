using MZ.Auth.Views;
using MZ.Core;
using Prism.Ioc;

namespace MZ.Auth
{
    public class AuthModule : MZModule
    {
        public AuthModule(IContainerExtension container) : base(container)
        {
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(MZRegionNames.UserLoginRegion, nameof(UserLoginView));
            _regionManager.RegisterViewWithRegion(MZRegionNames.UserRegisterRegion, nameof(UserRegisterView));
            _regionManager.RegisterViewWithRegion(MZRegionNames.UserInformationRegion, nameof(UserInformationView));
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<UserLoginView>();
            containerRegistry.RegisterForNavigation<UserRegisterView>();
            containerRegistry.RegisterForNavigation<UserInformationView>();
        }

        public override void InitializeEvent()
        {
        }
    }
}