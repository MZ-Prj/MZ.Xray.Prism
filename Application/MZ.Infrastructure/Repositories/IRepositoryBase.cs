using System.Collections.Generic;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
