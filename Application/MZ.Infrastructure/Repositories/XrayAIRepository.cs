using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace MZ.Infrastructure.Repositories
{
    /// <summary>
    /// AIOptionEntity (인공지능 옵션) 저장소
    /// 
    /// - AI 모델 옵션, 카테고리 등 관련 데이터베이스 접근 기능 제공
    /// - 단일 인스턴스, 카테고리 포함 CRUD, 카테고리 일괄 업데이트 등 지원
    /// </summary>
    [Repository]
    public class XrayAIOptionRepository : RepositoryBase<AIOptionEntity>, IXrayAIOptionRepository
    {
        public XrayAIOptionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<AIOptionEntity?> GetByIdSingleAsync()
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
            var aiOption = await _context.Set<AIOptionEntity>()
                                       .Include(a => a.Categories)
                                       .FirstOrDefaultAsync(a => a.Id == id);

            if (aiOption == null)
            {
                return null;
            }

            var existCategories = aiOption.Categories.ToDictionary(c => c.Id);

            foreach (var category in categories)
            {
                if (existCategories.TryGetValue(category.Id, out var exist))
                {
                    exist.Name = category.Name;
                    exist.Color = category.Color;
                    exist.Index = category.Index;
                    exist.IsUsing = category.IsUsing;
                    exist.Confidence = category.Confidence;

                    _context.Entry(exist).State = EntityState.Modified;
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
