using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

#nullable enable
namespace MZ.Infrastructure.Repositories
{
    /// <summary>
    /// XrayVision 이미지 저장소
    /// 
    /// - ImageEntity 테이블의 데이터 접근 담당
    /// - 페이지/기간별 조회, 오브젝트 탐지 포함 메소드 제공
    /// </summary>
    [Repository]
    public class XrayVisionImageRepository : RepositoryBase<ImageEntity>, IXrayVisionImageRepository
    {
        public XrayVisionImageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ICollection<ImageEntity>> GetByPageSize(int page, int size)
        {
            return await _context.Set<ImageEntity>()
                                 .Include(i => i.ObjectDetections)
                                 .OrderByDescending(i => i.Id)
                                 .Skip(page * size)
                                 .Take(size)
                                 .ToListAsync();
        }

        public async Task<ICollection<ImageEntity>> GetByDateTimeBetweenStartEnd(DateTime start, DateTime end)
        {
            return await _context.Set<ImageEntity>()
                                 .Include(i => i.ObjectDetections)
                                 .Where(i => i.CreateDate >= start && i.CreateDate <= end)
                                 .OrderByDescending(f => f.Id)
                                 .ToListAsync();
        }

        public async Task<ICollection<ImageEntity>> GetByDateTimeBetweenStartEndAndPageSize(DateTime start, DateTime end, int page, int size)
        {
            return await _context.Set<ImageEntity>()
                                 .Include(i => i.ObjectDetections)
                                 .Where(i => i.CreateDate >= start && i.CreateDate <= end)
                                 .OrderByDescending(i => i.Id)
                                 .Skip(page * size)
                                 .Take(size)
                                 .ToListAsync();
        }
    }

    /// <summary>
    /// XrayVision Calibration(캘리브레이션)  저장소
    /// 
    /// - CalibrationEntity 관련 DB 접근 담당
    /// - 사용자별 보정값 조회 제공
    /// </summary>
    [Repository]
    public class XrayVisionCalibrationRepository : RepositoryBase<CalibrationEntity>, IXrayVisionCalibrationRepository
    {
        public XrayVisionCalibrationRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<CalibrationEntity?> GetByUserIdAsync(int userId)
        {
            return await _context.Set<CalibrationEntity>()
                                 .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }

    /// <summary>
    /// XrayVision Filter(필터) 저장소
    /// 
    /// - FilterEntity 관련 DB 접근 담당
    /// - 사용자별 필터값 조회 제공
    /// </summary>
    [Repository]
    public class XrayVisionFilterRepository : RepositoryBase<FilterEntity>, IXrayVisionFilterRepository
    {
        public XrayVisionFilterRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<FilterEntity?> GetByUserIdAsync(int userId)
        {
            return await _context.Set<FilterEntity>()
                                 .FirstOrDefaultAsync(f => f.UserId == userId);
        }
    }

    /// <summary>
    /// XrayVision Material(재질/물질) 저장소
    /// 
    /// - MaterialEntity 관련 DB 접근 담당
    /// - 사용자별 재질 정보 및 MaterialControls 포함 조회 제공
    /// </summary>
    [Repository]
    public class XrayVisionMaterialRepository : RepositoryBase<MaterialEntity>, IXrayVisionMaterialRepository
    {
        public XrayVisionMaterialRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<MaterialEntity?> GetByUserIdAsync(int userId)
        {
            return await _context.Set<MaterialEntity>()
                                 .Include(m => m.MaterialControls) 
                                 .FirstOrDefaultAsync(m => m.UserId == userId);
        }

    }

    /// <summary>
    /// XrayVision MaterialControl 저장소
    /// 
    /// - MaterialControlEntity 관련 DB 접근 담당
    /// - Material별 Material 컨트롤 조회 제공
    /// </summary>
    [Repository]
    public class XrayVisionMaterialControlRepository : RepositoryBase<MaterialControlEntity>, IXrayVisionMaterialControlRepository
    {
        public XrayVisionMaterialControlRepository(AppDbContext context) : base(context)
        {
        }
    }

    /// <summary>
    /// XrayVision ZeffectControl 저장소
    /// 
    /// - ZeffectControlEntity 관련 DB 접근 담당
    /// - 사용자별 Zeffect 컨트롤 조회 제공
    /// </summary>
    [Repository]
    public class XrayVisionZeffectControlRepository : RepositoryBase<ZeffectControlEntity>, IXrayVisionZeffectControlRepository
    {
        public XrayVisionZeffectControlRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ICollection<ZeffectControlEntity>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<ZeffectControlEntity>()
                                 .Where(z => z.UserId == userId)
                                 .ToListAsync();
        }
    }


    /// <summary>
    /// XrayVision CurveControl 저장소
    /// 
    /// - CurveControlEntity 관련 DB 접근 담당
    /// - 사용자별 Curve 컨트롤 조회 제공
    /// </summary>
    [Repository]
    public class XrayVisionCurveControlRepository : RepositoryBase<CurveControlEntity>, IXrayVisionCurveControlRepository
    {
        public XrayVisionCurveControlRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ICollection<CurveControlEntity>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<CurveControlEntity>()
                                 .Where(z => z.UserId == userId)
                                 .ToListAsync();
        }

        public async Task DeleteByUserIdAsync(int userId)
        {
            var entity = _context.Set<CurveControlEntity>().Where(c => c.UserId == userId);
            _context.Set<CurveControlEntity>().RemoveRange(entity);
            await _context.SaveChangesAsync();
        }
    }
}
