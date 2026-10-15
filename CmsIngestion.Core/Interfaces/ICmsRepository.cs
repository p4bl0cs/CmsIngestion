using CmsIngestion.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsIngestion.Core.Interfaces
{
    public interface ICmsRepository
    {
        Task<CmsEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task AddAsync(CmsEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(CmsEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(CmsEntity entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
