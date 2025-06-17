using System.Reflection;
using System.Windows;
using MZ.Core;
using MZ.Dashboard;
using MZ.Dialog;
using MZ.Language;
using MZ.Loading;
using MZ.Producer.Engine;
using MZ.Resource;
using MZ.Sidebar;
using MZ.Splash;
using MZ.Toolbar;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;

namespace MZ.App
{
    public class MZBootstrapper : PrismBootstrapper
    {
        /// <summary>
        /// 
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
            // Initialize Database
            RegisterDatabaseServices(containerRegistry);

            // Repository
            RegisterRepositories(containerRegistry);

            // Service
            RegisterApplicationServices(containerRegistry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // Templates
            moduleCatalog.AddModule<LanguageModule>();
            moduleCatalog.AddModule<DialogModule>();
            moduleCatalog.AddModule<LoadingModule>();

            moduleCatalog.AddModule<SidebarModule>();
            moduleCatalog.AddModule<ToolbarModule>();

            // Clients (UI)
            moduleCatalog.AddModule<SplashModule>();
            moduleCatalog.AddModule<DashboardModule>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnInitialized()
        {
            // Theme
            ThemeService.Load(ThemeService.GetSystemTheme());

            // Language
            LanguageService.Load(LanguageService.GetSystemLanguage());

            // Build Version
            BuildVersionService.Load(Assembly.GetExecutingAssembly());
        }

        private void RegisterDatabaseServices(IContainerRegistry containerRegistry)
        {
            var appConfig = MZAppSettings.Configuration;

        }

        private void RegisterRepositories(IContainerRegistry containerRegistry)
        {
        }

        private void RegisterApplicationServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILoadingService, LoadingService>();
            containerRegistry.RegisterSingleton<IProducerService, ProducerService>();

            //custom dialog
            containerRegistry.RegisterDialogWindow<MZDialogWindow>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Exit()
        {

        }
    }
}
