using MZ.Domain.Entities;
using System.Threading.Tasks;

#nullable enable
namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// AppSetting 테이블 전용 Repository 인터페이스
    /// </summary>
    public interface IAppSettingRepository : IRepositoryBase<AppSettingEntity>
    {
        /// <summary>
        /// AppSetting 단일 조회 (비동기)
        /// </summary>
        Task<AppSettingEntity?> GetByIdSingleAsync();
    }
}
