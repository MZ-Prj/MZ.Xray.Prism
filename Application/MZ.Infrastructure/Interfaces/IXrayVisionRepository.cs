using MZ.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

#nullable enable
namespace MZ.Infrastructure.Interfaces
{
    public interface IXrayVisionImageRepository : IRepositoryBase<ImageEntity>
    {
        public Task<ICollection<ImageEntity>> GetImageByPageSize(int page, int size);
        public Task<ICollection<ImageEntity>> GetImageByDateTimeBetweenStartEnd(DateTime start, DateTime end);
        public Task<ICollection<ImageEntity>> GetImageByDateTimeBetweenStartEndAndPageSize(DateTime start, DateTime end, int page, int size);
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
}
