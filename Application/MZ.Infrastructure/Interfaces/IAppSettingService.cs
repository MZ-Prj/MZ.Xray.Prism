using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using System.Threading;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
    public interface IAppSettingService
    {

        Task<BaseResponse<AppSettingRole, AppSettingEntity>> Register(
            AppSettingRegisterRequest request,
            CancellationToken cancellationToken = default);

        Task<BaseResponse<AppSettingRole, AppSettingEntity>> GetAppSetting(
            CancellationToken cancellationToken = default);
    }
}
