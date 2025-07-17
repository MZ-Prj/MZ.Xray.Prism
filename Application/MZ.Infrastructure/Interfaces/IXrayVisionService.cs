
using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MZ.Infrastructure.Interfaces
{
    public interface IXrayVisionImageService
    {
        Task<BaseResponse<BaseRole, ImageEntity>> Save(ImageSaveRequest request);
        Task<BaseResponse<BaseRole, ICollection<ImageEntity>>> Load(ImageLoadRequest request);
    }

    public interface IXrayVisionFilterService
    {
    }

    public interface IXrayVisionMaterialService
    {
    }
}
