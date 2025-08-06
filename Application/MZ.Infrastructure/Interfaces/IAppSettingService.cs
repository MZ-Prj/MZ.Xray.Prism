using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// AppSetting(환경설정) 관련 서비스 인터페이스
    /// </summary>
    public interface IAppSettingService 
    {
        /// <summary>
        /// 환경설정 등록 또는 갱신 (비동기)
        /// </summary>
        Task<BaseResponse<AppSettingRole, AppSettingEntity>> Register(
            AppSettingRegisterRequest request);

        /// <summary>
        /// 환경설정 조회 (비동기)
        /// </summary>
        Task<BaseResponse<AppSettingRole, AppSettingEntity>> GetAppSetting();
    }
}
