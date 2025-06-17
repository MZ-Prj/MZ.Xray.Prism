using System.Reflection;
using System.Windows;
using Microsoft.EntityFrameworkCore;
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
            // Clients (UI)
            moduleCatalog.AddModule<SplashModule>();
            moduleCatalog.AddModule<DashboardModule>();

            // Templates
            moduleCatalog.AddModule<AuthModule>();
            moduleCatalog.AddModule<BlankModule>();
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

            // Business
            containerRegistry.Register<IUserService, UserService>();
            containerRegistry.Register<IAppSettingService, AppSettingService>();
        }

        private void RegisterRepositories(IContainerRegistry containerRegistry)
        {
            // Repository
            containerRegistry.Register<IUserRepository, UserRepository>();
            containerRegistry.Register<IUserSettingRepository, UserSettingRepository>();
            containerRegistry.Register<IAppSettingRepository, AppSettingRepository>();
        }

        private void RegisterApplicationServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILoadingService, LoadingService>();

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