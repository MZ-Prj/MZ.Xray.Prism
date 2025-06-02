using MZ.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
#nullable enable
    public interface IUserRepository : IRepositoryBase<UserEntity>
    {
        UserEntity GetUserByUsername(string username);
        void UpdateLastLoginDate(int id);
        Task<UserEntity?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<UserEntity> GetUserByUsernameAllRelationsAsync(string username, CancellationToken cancellationToken = default);
        Task UpdateLastLoginDateAsync(int id, CancellationToken cancellationToken = default);
    }

    public interface IUserSettingRepository : IRepositoryBase<UserSettingEntity>
    {
    }
}
