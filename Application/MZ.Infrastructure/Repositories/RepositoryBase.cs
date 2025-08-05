using System.Threading.Tasks;
using System.Collections.Generic;
using MZ.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

#nullable enable
namespace MZ.Infrastructure.Repositories
{
    /// <summary>
    /// 공통 리포지토리 베이스 클래스 (모든 엔티티의 CRUD 비동기 처리 공통 제공)
    /// </summary>
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly AppDbContext _context;

        public RepositoryBase(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// PK로 단일 조회 (비동기)
        /// </summary>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync([id]);
        }
        /// <summary>
        /// 전체 조회 (비동기)
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        /// <summary>
        /// 추가 (비동기)
        /// </summary>
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// 수정 (비동기)
        /// </summary>
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// 삭제 (비동기)
        /// </summary>
        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// PK로 삭제 (비동기)
        /// </summary>
        public async Task DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }
        /// <summary>
        /// 전체 삭제 (비동기)
        /// </summary>
        public async Task DeleteAllAsync()
        {
            var entities = await _context.Set<T>().ToListAsync();
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}