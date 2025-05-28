using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;

namespace MZ.Infrastructure.Repositories
{
    public class UserRepository : RepositoryBase<UserEntity>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public UserEntity GetUserByUsername(string username)
        {
            var user = _context.Set<UserEntity>().FirstOrDefault(u => u.Username == username);
            return user;
        }

        public async Task<UserEntity> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Set<UserEntity>().FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
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

        public async Task UpdateLastLoginDateAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Set<UserEntity>().FindAsync([id], cancellationToken);
            if (user != null)
            {
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public class UserSettingRepository : RepositoryBase<UserSettingEntity>, IUserSettingRepository
    {
        public UserSettingRepository(AppDbContext context) : base(context)
        {
        }
    }
}