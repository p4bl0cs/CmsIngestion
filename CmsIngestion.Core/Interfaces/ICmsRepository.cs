using CmsIngestion.Core.Entities;

namespace CmsIngestion.Core.Interfaces
{
    public interface ICmsRepository
    {
        Task<CmsEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task AddAsync(CmsEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(CmsEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(CmsEntity entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<CmsEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
