using MZ.Domain.Entities;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
#nullable enable
    public interface IUserRepository : IRepositoryBase<UserEntity>
    {
        UserEntity GetByUsername(string username);
        void UpdateLastLoginDate(int id);
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity> GetByUsernameAllRelationsAsync(string username);
        Task UpdateLastLoginDateAsync(int id);
    }

    public interface IUserSettingRepository : IRepositoryBase<UserSettingEntity>
    {
    }
}
