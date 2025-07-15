using MZ.Domain.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MZ.Infrastructure
{
    public class DatabaseService : IDatabaseService
    {
        public IUserService User { get; } 
        public IAppSettingService AppSetting { get; }

        private readonly CancellationTokenSource _cts;

        public DatabaseService(
            IUserService userService,
            IAppSettingService appSettingService)
        {
            _cts = new ();

            User = userService;
            AppSetting = appSettingService;
        }

        public async Task MakeAdmin()
        {
            string admin = "0000";
            bool isMake = await MakeUserAsync(admin, admin, _cts.Token);
            if (isMake)
            {
                await MakeAppSettingAsync(admin, _cts.Token);
            }
        }

        public async Task<bool> MakeUserAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var response = await User.Register(
                new UserRegisterRequest(username, password, password, $"{username}@test.com", UserRole.Admin),
                cancellationToken
            );
            return response.Success;
        }

        public async Task MakeAppSettingAsync(string username, CancellationToken cancellationToken = default)
        {
            await AppSetting.Register(
                new AppSettingRegisterRequest(username, false),
                cancellationToken
            );
        }
    }
}
