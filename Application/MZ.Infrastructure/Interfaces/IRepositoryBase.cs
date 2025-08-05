using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// Generic Repository의 기본 인터페이스
    /// </summary>
    public interface IRepositoryBase<T> where T : class
    {
        /// <summary>
        /// PK로 단일 조회 (비동기)
        /// </summary>
        Task<T?> GetByIdAsync(int id);
        /// <summary>
        /// 전체 조회 (비동기)
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();
        /// <summary>
        /// 추가 (비동기)
        /// </summary>
        Task AddAsync(T entity);
        /// <summary>
        /// 수정 (비동기)
        /// </summary>
        Task UpdateAsync(T entity);
        /// <summary>
        /// 삭제 (비동기)
        /// </summary>
        Task DeleteAsync(T entity);
        /// <summary>
        /// 전체 삭제 (비동기)
        /// </summary>
        Task DeleteAllAsync();
        /// <summary>
        /// PK로 삭제 (비동기)
        /// </summary>
        Task DeleteByIdAsync(int id);
    }
}
