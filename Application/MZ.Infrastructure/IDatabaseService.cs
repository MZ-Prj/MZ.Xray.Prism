using MZ.Infrastructure.Interfaces;
using System.Threading.Tasks;

namespace MZ.Infrastructure
{
    public interface IDatabaseService
    {
        IUserService User { get; }
        IAppSettingService AppSetting { get; }
        IXrayVisionImageService Image { get; }
        IXrayVisionFilterService Filter { get; }
        IXrayVisionCalibrationService Calibration { get; }
        IXrayVisionMaterialService Material { get; }
        IXrayAIOptionService AIOption { get; }
        Task MakeAdmin();
        Task<bool> MakeUserAsync(string username, string password);
        Task MakeAppSettingAsync(string username);
    }
}
