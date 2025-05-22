using System.Windows;
using MZ.Dashboard;
using MZ.Splash;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;

namespace MZ.App
{
    public class MZBootstrapper : PrismBootstrapper
    {
        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject CreateShell()
        {
            return null!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Register Service
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<SplashModule>();
            moduleCatalog.AddModule<DashboardModule>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnInitialized()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Exit()
        {

        }
    }
}
