using CmsIngestion.Application.DTOs;

namespace CmsIngestion.Application.Services
{
    public interface ICmsEventService
    {
        Task ProcessEventsAsync(IEnumerable<CmsEventDto> events, CancellationToken cancellationToken = default);

        Task<IEnumerable<CmsEventDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<CmsEventDto>> GetEntitiesForUserAsync(bool isAdmin, CancellationToken cancellationToken = default);

        Task DisableEntityAsync(string id, CancellationToken cancellationToken = default);
    }
}
