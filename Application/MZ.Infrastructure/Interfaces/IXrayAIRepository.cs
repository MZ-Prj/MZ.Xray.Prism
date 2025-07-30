using MZ.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace MZ.Infrastructure.Interfaces
{
    public interface IXrayAIOptionRepository : IRepositoryBase<AIOptionEntity>
    {
        Task<AIOptionEntity?> GetByIdSingleAsync();
        Task<AIOptionEntity?> GetByIdWithCategoriesAsync(int id);
        Task<AIOptionEntity?> UpdateCategoriesAsync(int id, ICollection<CategoryEntity> categories);
        Task<bool> IsOneAsync();
    }
}
