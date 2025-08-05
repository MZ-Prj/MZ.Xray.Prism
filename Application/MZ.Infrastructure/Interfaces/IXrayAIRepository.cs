using MZ.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// AI 옵션 관련 데이터 접근 레포지토리
    /// </summary>
    public interface IXrayAIOptionRepository : IRepositoryBase<AIOptionEntity>
    {
        /// <summary>
        /// 단일 AI 옵션 정보 조회 (비동기)
        /// </summary>
        Task<AIOptionEntity?> GetByIdSingleAsync();
        /// <summary>
        /// 카테고리 포함 AI 옵션 정보 조회 (비동기)
        /// </summary>
        Task<AIOptionEntity?> GetByIdWithCategoriesAsync(int id);
        /// <summary>
        /// AI 옵션의 카테고리 목록 갱신 (비동기)
        /// </summary>
        Task<AIOptionEntity?> UpdateCategoriesAsync(int id, ICollection<CategoryEntity> categories);
        /// <summary>
        /// 저장된 AI 옵션이 1개 존재하는지 확인 (비동기)
        /// </summary>
        Task<bool> IsOneAsync();
    }
}
