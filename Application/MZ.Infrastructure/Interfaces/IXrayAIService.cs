
using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
    public interface IXrayAIOptionService
    {
        Task<BaseResponse<BaseRole, AIOptionEntity>> Create(AIOptionCreateRequest request);
        Task<BaseResponse<BaseRole, AIOptionEntity>> Save(AIOptionSaveRequest request);
    }
}
