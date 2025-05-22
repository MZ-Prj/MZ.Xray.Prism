using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MZ.Infrastructure.Repositories
{
#nullable enable
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly DbConfig _config;

        public RepositoryBase(DbConfig config)
        {
            _config = config;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _config.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _config.Set<T>().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _config.Set<T>().AddAsync(entity);
            await _config.SaveChangesAsync();
        }

        async Task<T> IRepositoryBase<T>.GetByIdAsync(int id)
        {
            return (await GetByIdAsync(id))!;
        }
    }
}
