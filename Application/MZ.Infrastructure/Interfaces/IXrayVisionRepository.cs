using MZ.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

#nullable enable
namespace MZ.Infrastructure.Interfaces
{
    public interface IXrayVisionImageRepository : IRepositoryBase<ImageEntity>
    {
        public Task<ICollection<ImageEntity>> GetByPageSize(int page, int size);
        public Task<ICollection<ImageEntity>> GetByDateTimeBetweenStartEnd(DateTime start, DateTime end);
        public Task<ICollection<ImageEntity>> GetByDateTimeBetweenStartEndAndPageSize(DateTime start, DateTime end, int page, int size);
    }
    public interface IXrayVisionCalibrationRepository : IRepositoryBase<CalibrationEntity>
    {
        public Task<CalibrationEntity?> GetByUserIdAsync(int userId);
    }

    public interface IXrayVisionFilterRepository : IRepositoryBase<FilterEntity>
    {
        public Task<FilterEntity?> GetByUserIdAsync(int userId);
    }

    public interface IXrayVisionMaterialRepository : IRepositoryBase<MaterialEntity>
    {
        public Task<MaterialEntity?> GetByUserIdAsync(int userId);
    }

    public interface IXrayVisionZeffectControlRepository : IRepositoryBase<ZeffectControlEntity>
    {
        public Task<ICollection<ZeffectControlEntity>> GetByUserIdAsync(int userId);
    }
    
}
