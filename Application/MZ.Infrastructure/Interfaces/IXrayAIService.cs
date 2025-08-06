using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// Xray AI 옵션 서비스 인터페이스  
    /// AI 옵션 데이터의 생성, 저장, 조회, 삭제 등을 담당
    /// </summary>
    public interface IXrayAIOptionService
    {
        /// <summary>
        /// 새로운 AI 옵션 생성 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, AIOptionEntity>> Create(AIOptionCreateRequest request);
        /// <summary>
        /// AI 옵션의 카테고리 정보 저장 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, AIOptionEntity>> Save(AIOptionSaveRequest request);

        /// <summary>
        /// 단일 AI 옵션 레코드 존재 여부 확인 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, bool>> ExistOneRecord();
        /// <summary>
        /// AI 옵션 데이터 불러오기 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, AIOptionEntity>> Load();
        /// <summary>
        /// AI 옵션 데이터 삭제 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, bool>> Delete();

    }
}
