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
        Task<BaseResponse<BaseRole, ICollection<ImageLoadResponse>>> Load(ImageLoadRequest request);
        Task<BaseResponse<BaseRole, ICollection<ImageEntity>>> Load(ReportImageLoadRequest request);
    }

    public interface IXrayVisionFilterService 
    {
        Task<BaseResponse<BaseRole, FilterEntity>> Save(FilterSaveRequest request);
        Task<BaseResponse<BaseRole, FilterEntity>> Load(FilterLoadRequest request);
    }
    public interface IXrayVisionCalibrationService
    {
        Task<BaseResponse<BaseRole, CalibrationEntity>> Save(CalibrationSaveRequest request);
        Task<BaseResponse<BaseRole, CalibrationEntity>> Load(CalibrationLoadRequest request);
    }

    public interface IXrayVisionMaterialService
    {
        Task<BaseResponse<BaseRole, MaterialEntity>> Save(MaterialSaveRequest request);
        Task<BaseResponse<BaseRole, MaterialEntity>> Load(MaterialLoadRequest request);
    }
    public interface IXrayVisionZeffectControlService
    {
        Task<BaseResponse<BaseRole, ICollection<ZeffectControlEntity>>> Save(ZeffectControlSaveRequest request);
        Task<BaseResponse<BaseRole, ICollection<ZeffectControlEntity>>> Load(ZeffectControlLoadRequest request);
    }
}
