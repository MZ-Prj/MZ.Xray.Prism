using Microsoft.EntityFrameworkCore;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Repositories
{
    public class XrayVisionImageRepository : RepositoryBase<ImageEntity>, IXrayVisionImageRepository
    {
        public XrayVisionImageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ICollection<ImageEntity>> GetImageByPageSize(int page, int size)
        {
            return await _context.Set<ImageEntity>()
                                 .Include(f => f.ObjectDetections)
                                 .OrderByDescending(f => f.Id)
                                 .Skip(page * size)
                                 .Take(size)
                                 .ToListAsync();
        }

        public async Task<ICollection<ImageEntity>> GetImageByDateTimeBetweenStartEnd(DateTime start, DateTime end)
        {
            return await _context.Set<ImageEntity>()
                                 .Include(f => f.ObjectDetections)
                                 .Where(f => f.CreateDate >= start && f.CreateDate <= end)
                                 .OrderByDescending(f => f.Id)
                                 .ToListAsync();
        }

        public async Task<ICollection<ImageEntity>> GetImageByDateTimeBetweenStartEndAndPageSize(DateTime start, DateTime end, int page, int size)
        {
            return await _context.Set<ImageEntity>()
                                 .Include(f => f.ObjectDetections)
                                 .Where(f => f.CreateDate >= start && f.CreateDate <= end)
                                 .OrderByDescending(f => f.Id)
                                 .Skip(page * size)
                                 .Take(size)
                                 .ToListAsync();
        }
    }

    public class XrayVisionFilterRepository : RepositoryBase<FilterEntity>, IXrayVisionFilterRepository
    {
        public XrayVisionFilterRepository(AppDbContext context) : base(context)
        {
        }
    }

    public class XrayVisionMaterialRepository : RepositoryBase<MaterialEntity>, IXrayVisionMaterialRepository
    {
        public XrayVisionMaterialRepository(AppDbContext context) : base(context)
        {
        }
    }
}
