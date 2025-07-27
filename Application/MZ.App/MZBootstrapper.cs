using System.Reflection;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using MZ.AI.Engine;
using MZ.Auth;
using MZ.Blank;
using MZ.Core;
using MZ.Dashboard;
using MZ.Dialog;
using MZ.Infrastructure;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Repositories;
using MZ.Infrastructure.Services;
using MZ.Infrastructure.Sessions;
using MZ.Language;
using MZ.Loading;
using MZ.Resource;
using MZ.Splash;
using MZ.WindowDialog;
using MZ.Xray.Engine;
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

            // Service
            RegisterApplicationServices(containerRegistry);
            RegisterUIService(containerRegistry);
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
            moduleCatalog.AddModule<BlankModule>();
            moduleCatalog.AddModule<LanguageModule>();
            moduleCatalog.AddModule<DialogModule>();
            moduleCatalog.AddModule<WindowDialogModule>();
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

        private void RegisterDatabaseServices(IContainerRegistry containerRegistry)
        {
            var appConfig = MZAppSettings.Configuration;

            // DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite(appConfig["Database:Path"])
                    .EnableSensitiveDataLogging()
                    .Options;

            containerRegistry.RegisterInstance(options);
            containerRegistry.Register<AppDbContext>();

            // DatabaseService
            containerRegistry.RegisterSingleton<IDatabaseService, DatabaseService>();

            // Session
            containerRegistry.RegisterSingleton<IUserSession, UserSession>();

            // Repository
            containerRegistry.Register<IUserRepository, UserRepository>();
            containerRegistry.Register<IUserSettingRepository, UserSettingRepository>();
            containerRegistry.Register<IAppSettingRepository, AppSettingRepository>();
            containerRegistry.Register<IXrayVisionImageRepository, XrayVisionImageRepository>();
            containerRegistry.Register<IXrayVisionCalibrationRepository, XrayVisionCalibrationRepository>();
            containerRegistry.Register<IXrayVisionFilterRepository, XrayVisionFilterRepository>();
            containerRegistry.Register<IXrayVisionMaterialRepository, XrayVisionMaterialRepository>();

            // Business
            containerRegistry.Register<IUserService, UserService>();
            containerRegistry.Register<IAppSettingService, AppSettingService>();
            containerRegistry.Register<IXrayVisionImageService, XrayVisionImageService>();
            containerRegistry.Register<IXrayVisionFilterService, XrayVisionFilterService>();
            containerRegistry.Register<IXrayVisionCalibrationService, XrayVisionCalibrationService>();
            containerRegistry.Register<IXrayVisionMaterialService, XrayVisionMaterialService>();
        }

        private void RegisterApplicationServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILoadingService, LoadingService>();
            containerRegistry.RegisterSingleton<IWindowDialogService, WindowDialogService>();
            containerRegistry.RegisterSingleton<IXrayService, XrayService>();
            containerRegistry.RegisterSingleton<IAIService, AIService>();
        }

        private void RegisterUIService(IContainerRegistry containerRegistry)
        {
            //mahapp : custom dialog
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