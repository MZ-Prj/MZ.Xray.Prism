using MZ.Domain.Entities;
using System.Threading.Tasks;

#nullable enable
namespace MZ.Infrastructure.Interfaces
{
    public interface IAppSettingRepository : IRepositoryBase<AppSettingEntity>
    {
        Task<AppSettingEntity?> GetByIdSingleAsync();
    }
}
