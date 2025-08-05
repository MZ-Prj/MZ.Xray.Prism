using MZ.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

#nullable enable
namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// XrayVision 이미지 관련 저장소 인터페이스
    /// </summary>
    public interface IXrayVisionImageRepository : IRepositoryBase<ImageEntity>
    {
        /// <summary>
        /// 페이지/사이즈 기준으로 이미지 조회 (비동기)
        /// </summary>
        public Task<ICollection<ImageEntity>> GetByPageSize(int page, int size);
        /// <summary>
        /// 날짜 기준 이미지 조회 (비동기)
        /// </summary>
        public Task<ICollection<ImageEntity>> GetByDateTimeBetweenStartEnd(DateTime start, DateTime end);
        /// <summary>
        /// 날짜 및 페이지/사이즈 기준 이미지 조회 (비동기)
        /// </summary>
        public Task<ICollection<ImageEntity>> GetByDateTimeBetweenStartEndAndPageSize(DateTime start, DateTime end, int page, int size);
    }
    /// <summary>
    /// XrayVision Calibration(캘리브레이션) 저장소 인터페이스
    /// </summary>
    public interface IXrayVisionCalibrationRepository : IRepositoryBase<CalibrationEntity>
    {
        /// <summary>
        /// 사용자 ID로 캘리브레이션 정보 조회 (비동기)
        /// </summary>
        public Task<CalibrationEntity?> GetByUserIdAsync(int userId);
    }

    /// <summary>
    /// XrayVision Filter(필터) 저장소 인터페이스
    /// </summary>
    public interface IXrayVisionFilterRepository : IRepositoryBase<FilterEntity>
    {
        /// <summary>
        /// 사용자 ID로 필터 정보 조회 (비동기)
        /// </summary>
        public Task<FilterEntity?> GetByUserIdAsync(int userId);
    }
    /// <summary>
    /// XrayVision Material(재질/물질) 저장소 인터페이스
    /// </summary>
    public interface IXrayVisionMaterialRepository : IRepositoryBase<MaterialEntity>
    {
        /// <summary>
        /// 사용자 ID로 재질 정보 조회 (비동기)
        /// </summary>
        public Task<MaterialEntity?> GetByUserIdAsync(int userId);
    }
    /// <summary>
    /// XrayVision ZeffectControl 저장소 인터페이스
    /// </summary>
    public interface IXrayVisionZeffectControlRepository : IRepositoryBase<ZeffectControlEntity>
    {
        /// <summary>
        /// 사용자 ID로 ZeffectControl 리스트 조회 (비동기)
        /// </summary>
        public Task<ICollection<ZeffectControlEntity>> GetByUserIdAsync(int userId);
    }
    
}
