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
        public IXrayVisionImageService Image { get; }
        public IXrayVisionFilterService Filter { get; }
        public IXrayVisionMaterialService Material { get; }
        public IXrayVisionCalibrationService Calibration { get; }
        public IXrayAIOptionService AIOption { get; }

        /// <summary>
        /// cancel 시 token 제어 필요시
        /// </summary>
        private readonly CancellationTokenSource _cts;

        public DatabaseService(
            IUserService userService,
            IAppSettingService appSettingService,
            IXrayVisionImageService xrayVisionImageService,
            IXrayVisionFilterService xrayVisionFilterService,
            IXrayVisionMaterialService xrayVisionMaterialService,
            IXrayVisionCalibrationService xrayVisionCalibrationService,
            IXrayAIOptionService xrayAIOptionService)
        {
            _cts = new ();

            User = userService;
            AppSetting = appSettingService;
            Image = xrayVisionImageService;
            Filter = xrayVisionFilterService;
            Material = xrayVisionMaterialService;
            Calibration = xrayVisionCalibrationService;
            AIOption = xrayAIOptionService;
        }

        public async Task MakeAdmin()
        {
            string admin = "0000";
            bool isMake = await MakeUserAsync(admin, admin);
            if (isMake)
            {
                await MakeAppSettingAsync(admin);
            }
        }
        public async Task MakeAppSettingAsync(string username)
        {
            await AppSetting.Register(
                new AppSettingRegisterRequest(username, false)
            );
        }

        public async Task<bool> MakeUserAsync(string username, string password)
        {
            var response = await User.Register(
                new UserRegisterRequest(username, password, password, $"{username}@test.com", UserRole.Admin)
            );
            return response.Success;
        }

    }
}
