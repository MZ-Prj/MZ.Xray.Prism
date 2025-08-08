using System;
using System.Reflection;
using System.Windows;
using DryIoc;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        protected override DependencyObject CreateShell()
        {
            return null!;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Config
            RegisterConfigurations(containerRegistry);

            // Database
            RegisterDatabaseServices(containerRegistry);

            // Service
            RegisterApplicationServices(containerRegistry);
            RegisterUIServices(containerRegistry);
        }

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

        private void RegisterDatabaseServices(IContainerRegistry containerRegistry)
        {
            // 
            var configuration = containerRegistry.GetContainer().Resolve<IConfiguration>();

            // DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite(configuration["Database:Path"])
                    .EnableSensitiveDataLogging()
                    .Options;

            containerRegistry.RegisterInstance(options);
            containerRegistry.Register<AppDbContext>();

            // DatabaseService
            containerRegistry.RegisterSingleton<IDatabaseService, DatabaseService>();

            // Session
            containerRegistry.RegisterSingleton<IUserSession, UserSession>();

            // Repository
            RegisterByAttribute(containerRegistry, typeof(RepositoryBase<>).Assembly);

            // Business
            RegisterByAttribute(containerRegistry, typeof(ServiceBase).Assembly);

        }

        private void RegisterApplicationServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILoadingService, LoadingService>();
            containerRegistry.RegisterSingleton<IWindowDialogService, WindowDialogService>();
            containerRegistry.RegisterSingleton<IXrayService, XrayService>();
            containerRegistry.RegisterSingleton<IAIService, AIService>();

        }

        private void RegisterUIServices(IContainerRegistry containerRegistry)
        {
            //mahapp : custom dialog
            containerRegistry.RegisterDialogWindow<MZDialogMetroWindowChrome>();

            containerRegistry.RegisterInstance<IDialogCoordinator>(DialogCoordinator.Instance);
        }

        public void Exit()
        {

        }

        public void RegisterByAttribute(IContainerRegistry containerRegistry, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract)
                {
                    if (type.GetCustomAttribute<RepositoryAttribute>() != null || type.GetCustomAttribute<ServiceAttribute>() != null)
                    {
                        var interfaceName = "I" + type.Name;
                        var iface = type.GetInterface(interfaceName);

                        if (iface != null)
                        {
                            containerRegistry.Register(iface, type);
                        }
                    }
                }
            }
        }
    }
}