using MZ.Domain.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MZ.Infrastructure
{
    /// <summary>
    /// 데이터베이스 서비스의 엔트리 포인트 클레스
    /// 
    /// - 각종 비즈니스 서비스(User, AppSetting, Image 등)에 대한 접근 포인트 역할을 수행
    /// - 시스템 초기 데이터 생성(Admin, User, AppSetting 등) 유틸리티 메서드 제공
    /// - DI(의존성 주입) 컨테이너에서 하나의 서비스로 등록해 사용 가능
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        public IUserService User { get; } 
        public IAppSettingService AppSetting { get; }
        public IXrayVisionImageService Image { get; }
        public IXrayVisionFilterService Filter { get; }
        public IXrayVisionMaterialService Material { get; }
        public IXrayVisionCalibrationService Calibration { get; }
        public IXrayVisionZeffectControlService ZeffectControl { get; }
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
            IXrayVisionZeffectControlService xrayVisionZeffectControlService,
            IXrayAIOptionService xrayAIOptionService)
        {
            _cts = new ();

            User = userService;
            AppSetting = appSettingService;
            Image = xrayVisionImageService;
            Filter = xrayVisionFilterService;
            Material = xrayVisionMaterialService;
            Calibration = xrayVisionCalibrationService;
            ZeffectControl = xrayVisionZeffectControlService;
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
