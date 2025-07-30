using MZ.Infrastructure.Interfaces;
using System.Threading.Tasks;

namespace MZ.Infrastructure
{
    public interface IDatabaseService
    {
        public IUserService User { get; }
        public IAppSettingService AppSetting { get; }
        public IXrayVisionImageService Image { get; }
        public IXrayVisionFilterService Filter { get; }
        public IXrayVisionCalibrationService Calibration { get; }
        public IXrayVisionMaterialService Material { get; }
        public IXrayAIOptionService AIOption { get; }

        public Task MakeAdmin();
        public Task<bool> MakeUserAsync(string username, string password);
        public Task MakeAppSettingAsync(string username);
    }
}
