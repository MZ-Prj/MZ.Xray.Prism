using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// Xray Vision 이미지 서비스 인터페이스  
    /// 이미지 데이터의 저장 및 조회 관련 기능 제공
    /// </summary>
    public interface IXrayVisionImageService
    {
        /// <summary>
        /// 이미지 데이터 저장
        /// </summary>
        Task<BaseResponse<BaseRole, ImageEntity>> Save(ImageSaveRequest request);
        /// <summary>
        /// 페이징, 검색 조건에 따른 이미지 리스트 조회 (일반)
        /// </summary>
        Task<BaseResponse<BaseRole, ICollection<ImageLoadResponse>>> Load(ImageLoadRequest request);

        /// <summary>
        /// 기간 조건에 따른 이미지 Entity 컬렉션 조회 (리포트용)
        /// </summary>
        Task<BaseResponse<BaseRole, ICollection<ImageEntity>>> Load(ReportImageLoadRequest request);
    }
    /// <summary>
    /// Xray Vision Filter(영상 조정) 서비스 인터페이스  
    /// Filter(밝기, 대비 등) 설정 데이터 저장 및 불러오기 기능 제공
    /// </summary>
    public interface IXrayVisionFilterService 
    {
        /// <summary>
        /// Filter(영상 조정값) 저장
        /// </summary>
        Task<BaseResponse<BaseRole, FilterEntity>> Save(FilterSaveRequest request);

        /// <summary>
        /// Filter(영상 조정값) 불러오기
        /// </summary>
        Task<BaseResponse<BaseRole, FilterEntity>> Load(FilterLoadRequest request);
    }
    /// <summary>
    /// Xray Vision Calibration 서비스 인터페이스  
    /// 보정값 저장 및 불러오기 기능 제공
    /// </summary>
    public interface IXrayVisionCalibrationService
    {
        /// <summary>
        /// Calibration 저장
        /// </summary>
        Task<BaseResponse<BaseRole, CalibrationEntity>> Save(CalibrationSaveRequest request);
        /// <summary>
        /// Calibration 불러오기
        /// </summary>
        Task<BaseResponse<BaseRole, CalibrationEntity>> Load(CalibrationLoadRequest request);
    }
    /// <summary>
    /// Xray Vision Material 서비스 인터페이스  
    /// Material 데이터 저장 및 조회 기능 제공
    /// </summary>
    public interface IXrayVisionMaterialService
    {
        /// <summary>
        /// Material 데이터 저장
        /// </summary>
        Task<BaseResponse<BaseRole, MaterialEntity>> Save(MaterialSaveRequest request);
        /// <summary>
        /// Material 데이터 불러오기
        /// </summary>
        Task<BaseResponse<BaseRole, MaterialEntity>> Load(MaterialLoadRequest request);
    }

    /// <summary>
    /// Xray Vision Zeffect 서비스 인터페이스  
    /// Zeffect 제어 정보 저장 및 조회 기능 제공
    /// </summary>
    public interface IXrayVisionZeffectControlService
    {
        /// <summary>
        /// Zeffect 제어 정보 저장
        /// </summary>
        Task<BaseResponse<BaseRole, ICollection<ZeffectControlEntity>>> Save(ZeffectControlSaveRequest request);
        /// <summary>
        /// Zeffect 제어 정보 불러오기
        /// </summary>
        Task<BaseResponse<BaseRole, ICollection<ZeffectControlEntity>>> Load(ZeffectControlLoadRequest request);
    }


    /// <summary>
    /// Xray Vision Curve 서비스 인터페이스  
    /// Curve 제어 정보 저장 및 조회 기능 제공
    /// </summary>
    public interface IXrayVisionCurveControlService
    {
        /// <summary>
        /// Curve 제어 정보 저장
        /// </summary>
        Task<BaseResponse<BaseRole, ICollection<CurveControlEntity>>> Save(CurveControlSaveRequest request);
        /// <summary>
        /// Curve 제어 정보 불러오기
        /// </summary>
        Task<BaseResponse<BaseRole, ICollection<CurveControlEntity>>> Load(CurveControlLoadRequest request);
    }
}
