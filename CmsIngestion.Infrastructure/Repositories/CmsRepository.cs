using CmsIngestion.Core.Entities;
using CmsIngestion.Core.Interfaces;
using CmsIngestion.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsIngestion.Infrastructure.Repositories
{
    public class CmsRepository : ICmsRepository
    {
        private readonly AppDbContext _context;

        public CmsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CmsEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.CmsEntities.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(CmsEntity entity, CancellationToken cancellationToken = default)
        {
            await _context.CmsEntities.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(CmsEntity entity, CancellationToken cancellationToken = default)
        {
            _context.CmsEntities.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CmsEntity entity, CancellationToken cancellationToken = default)
        {
            _context.CmsEntities.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
