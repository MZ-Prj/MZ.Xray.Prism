using System.Reflection;
using System.Windows;
using MZ.Auth;
using MZ.Dashboard;
using MZ.Dialog;
using MZ.Infrastructure;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Repositories;
using MZ.Infrastructure.Sessions;
using MZ.Language;
using MZ.Loading;
using MZ.Resource;
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
            // Register Service
            containerRegistry.RegisterSingleton<DatabaseService>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // Clients (UI)
            moduleCatalog.AddModule<SplashModule>();
            moduleCatalog.AddModule<DashboardModule>();

            // Templates
            moduleCatalog.AddModule<AuthModule>();
            moduleCatalog.AddModule<LanguageModule>();
            moduleCatalog.AddModule<DialogModule>();
            moduleCatalog.AddModule<LoadingModule>();
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

        /// <summary>
        /// 
        /// </summary>
        public void Exit()
        {

        }
    }
}
