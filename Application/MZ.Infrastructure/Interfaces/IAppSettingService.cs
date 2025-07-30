using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
    public interface IAppSettingService 
    {

        Task<BaseResponse<AppSettingRole, AppSettingEntity>> Register(
            AppSettingRegisterRequest request);

        Task<BaseResponse<AppSettingRole, AppSettingEntity>> GetAppSetting();
    }
}
