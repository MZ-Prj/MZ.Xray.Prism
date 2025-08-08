using System;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
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
            // Config
            RegisterConfigurations(containerRegistry);

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
            var configuration = Container.Resolve<IConfiguration>();
            BuildVersionService.Load(Assembly.GetExecutingAssembly(), configuration["Build:Version"]);
        }

        private void RegisterConfigurations(IContainerRegistry containerRegistry)
        {
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .Build();

            containerRegistry.RegisterInstance<IConfiguration>(configuration);
        }

        private void RegisterApplicationServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILoadingService, LoadingService>();
            containerRegistry.RegisterSingleton<IProducerService, ProducerService>();

            //custom dialog
            containerRegistry.RegisterDialogWindow<MZDialogMetroWindowChrome>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Exit()
        {

        }
    }
}
