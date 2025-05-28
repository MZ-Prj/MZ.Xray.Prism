using Microsoft.Extensions.DependencyInjection;
using MZ.Domain.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Repositories;
using MZ.Infrastructure.Services;
using MZ.Infrastructure.Sessions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MZ.Infrastructure
{
    public class DatabaseService
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private CancellationTokenSource _cts;
        public DatabaseService()
        {
            _cts = new CancellationTokenSource();
        }

        public void InitializeCore()
        {
            ServiceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>()

                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserSettingRepository, UserSettingRepository>()
                .AddScoped<IAppSettingRepository, AppSettingRepository>()

                .AddScoped<IUserService, UserService>()
                .AddScoped<IAppSettingService, AppSettingService>()

                .AddSingleton<IUserSession, UserSession>()
            .BuildServiceProvider();
        }

        public async Task InitializeModelAsync()
        {
            string admin = "0000";
            await MakeUserAsync(admin, admin, _cts.Token);
            await MakeAppSettingAsync(admin, _cts.Token);
        }

        public async Task MakeUserAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var scope = ServiceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            await userService.Register(
                new UserRegisterRequest(username, password, password, $"{username}@test.com", UserRole.Admin),
                cancellationToken
            );
        }

        public async Task MakeAppSettingAsync(string username, CancellationToken cancellationToken = default)
        {
            var scope = ServiceProvider.CreateScope();
            var appSettingService = scope.ServiceProvider.GetRequiredService<IAppSettingService>();

            await appSettingService.Register(
                new AppSettingRegisterRequest(username, false),
                cancellationToken
            );
        }
    }
}
