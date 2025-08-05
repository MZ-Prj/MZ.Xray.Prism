using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System.Threading.Tasks;

#nullable enable
namespace MZ.Infrastructure.Repositories
{
    /// <summary>
    /// AppSetting(앱 환경설정) 엔티티용 저장소
    /// </summary>
    [Repository]
    public class AppSettingRepository : RepositoryBase<AppSettingEntity>, IAppSettingRepository
    {
        public AppSettingRepository(AppDbContext config) : base(config)
        {
        }

        public async Task<AppSettingEntity?> GetByIdSingleAsync()
        {
            return await _context.Set<AppSettingEntity>()
                                 .SingleOrDefaultAsync();
        }
    }
}
