using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;

namespace MZ.Infrastructure.Repositories
{
    public class AppSettingRepository : RepositoryBase<AppSettingEntity>, IAppSettingRepository
    {
        public AppSettingRepository(AppDbContext config) : base(config)
        {
        }
    }
}
