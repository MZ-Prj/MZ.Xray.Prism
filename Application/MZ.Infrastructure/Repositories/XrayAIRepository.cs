using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace MZ.Infrastructure.Repositories
{
    public class XrayAIOptionRepository : RepositoryBase<AIOptionEntity>, IXrayAIOptionRepository
    {
        public XrayAIOptionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<AIOptionEntity?> GetByIdAsync(int id)
        {
            return await _context.Set<AIOptionEntity>()
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<AIOptionEntity?> GetByIdAsync()
        {
            return await _context.Set<AIOptionEntity>()
                                 .Include(a => a.Categories)
                                 .SingleOrDefaultAsync();
        }

        public async Task<AIOptionEntity?> GetByIdWithCategoriesAsync(int id)
        {
            return await _context.Set<AIOptionEntity>()
                                 .Include(m => m.Categories)
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<AIOptionEntity?> UpdateCategoriesAsync(int id, ICollection<CategoryEntity> categories)
        {
            var option = await _context.Set<AIOptionEntity>()
                                       .Include(a => a.Categories)
                                       .FirstOrDefaultAsync(a => a.Id == id);

            if (option == null)
            {
                return null;
            }

            var existCategories = option.Categories.ToDictionary(c => c.Id);

            foreach (var category in categories)
            {
                if (existCategories.TryGetValue(category.Id, out var exist))
                {
                    _context.Entry(exist).CurrentValues.SetValues(category);
                }
            }

            await _context.SaveChangesAsync();
            return await GetByIdWithCategoriesAsync(id);
        }
        public async Task<bool> IsOneAsync()
        {
            var count = await _context.Set<AIOptionEntity>().CountAsync();
            return count == 1;
        }
    }

}
