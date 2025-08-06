using MZ.Domain.Entities;
using System.Threading.Tasks;

#nullable enable
namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// 사용자(User) 관련 데이터 접근 레포지토리
    /// </summary>
    public interface IUserRepository : IRepositoryBase<UserEntity>
    {
        /// <summary>
        /// 사용자명으로 사용자 단일 조회
        /// </summary>
        UserEntity GetByUsername(string username);
        /// <summary>
        /// 마지막 로그인 시간 갱신
        /// </summary>
        void UpdateLastLoginDate(int id);
        /// <summary>
        /// 사용자명으로 사용자 단일 조회 (비동기)
        /// </summary>
        Task<UserEntity?> GetByUsernameAsync(string username);
        /// <summary>
        /// 사용자명으로 모든 연관 정보까지 포함 조회 (비동기)
        /// </summary>
        Task<UserEntity> GetByUsernameAllRelationsAsync(string username);
        /// <summary>
        /// 마지막 로그인 시간 갱신 (비동기)
        /// </summary>
        Task UpdateLastLoginDateAsync(int id);
    }

    /// <summary>
    /// 사용자 설정(UserSetting) 관련 데이터 접근 레포지토리
    /// </summary>
    public interface IUserSettingRepository : IRepositoryBase<UserSettingEntity>
    {
    }
}
