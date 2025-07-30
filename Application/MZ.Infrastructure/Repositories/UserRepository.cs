using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;

namespace MZ.Infrastructure.Repositories
{
    [Repository]
    public class UserRepository : RepositoryBase<UserEntity>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public UserEntity GetByUsername(string username)
        {
            var user = _context.Set<UserEntity>().FirstOrDefault(u => u.Username == username);
            return user;
        }

        public async Task<UserEntity> GetByUsernameAsync(string username)
        {
            return await _context.Set<UserEntity>().FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserEntity> GetByUsernameAllRelationsAsync(string username)
        {
            return await _context.Set<UserEntity>()
                                 .Include(u => u.UserSetting)
                                    .ThenInclude(us => us.Buttons)
                                 .Include(u => u.Calibration)
                                 .Include(u => u.Filter)
                                 .Include(u => u.Material)
                                 .FirstOrDefaultAsync(u => u.Username == username);
        }

        public void UpdateLastLoginDate(int id)
        {
            var user = _context.Set<UserEntity>().Find(id);
            if (user != null)
            {
                user.LastLoginDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public async Task UpdateLastLoginDateAsync(int id)
        {
            var user = await _context.Set<UserEntity>().FindAsync([id]);
            if (user != null)
            {
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }

    [Repository]
    public class UserSettingRepository : RepositoryBase<UserSettingEntity>, IUserSettingRepository
    {
        public UserSettingRepository(AppDbContext context) : base(context)
        {
        }
    }
}