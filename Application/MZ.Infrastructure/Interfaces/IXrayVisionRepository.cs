using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
    public interface IXrayVisionImageRepository : IRepositoryBase<ImageEntity>
    {
        public Task<ICollection<ImageEntity>> GetImageByPageSize(int page, int size);
        public Task<ICollection<ImageEntity>> GetImageByDateTimeBetweenStartEnd(DateTime start, DateTime end);
        public Task<ICollection<ImageEntity>> GetImageByDateTimeBetweenStartEndAndPageSize(DateTime start, DateTime end, int page, int size);
    }

    public interface IXrayVisionFilterRepository : IRepositoryBase<FilterEntity>
    {
    }

    public interface IXrayVisionMaterialRepository : IRepositoryBase<MaterialEntity>
    {
    }
}
