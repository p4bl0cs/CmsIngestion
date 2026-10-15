using CmsIngestion.Application.DTOs;
using CmsIngestion.Core.Entities;
using CmsIngestion.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsIngestion.Application.Services
{
    public class CmsEventService : ICmsEventService
    {
        private readonly ICmsRepository _repository;
        private readonly ILogger<CmsEventService> _logger;

        public CmsEventService(ICmsRepository repository, ILogger<CmsEventService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task ProcessEventsAsync(IEnumerable<CmsEventDto> events, CancellationToken cancellationToken = default)
        {
            foreach (var evt in events)
            {
                try
                {
                    var existingEntity = await _repository.GetByIdAsync(evt.Id, cancellationToken);

                    switch (evt.Type.ToLowerInvariant())
                    {
                        case "publish":
                            if (existingEntity == null)
                            {
                                var newEntity = new CmsEntity
                                {
                                    Id = evt.Id,
                                    Version = evt.Version,
                                    PayloadJson = evt.Payload?.ToJsonString() ?? "{}",
                                    IsDisabled = false,
                                    CreatedAt = evt.Timestamp != default ? evt.Timestamp : DateTimeOffset.UtcNow,
                                    UpdatedAt = DateTimeOffset.UtcNow
                                };
                                await _repository.AddAsync(newEntity, cancellationToken);
                            }
                            else if (evt.Version >= existingEntity.Version)
                            {
                                existingEntity.Version = evt.Version;
                                existingEntity.PayloadJson = evt.Payload?.ToJsonString() ?? existingEntity.PayloadJson;
                                existingEntity.IsDisabled = false;
                                existingEntity.UpdatedAt = DateTimeOffset.UtcNow;
                                await _repository.UpdateAsync(existingEntity, cancellationToken);
                            }
                            break;

                        case "unpublish":
                            if (existingEntity != null)
                            {
                                existingEntity.IsDisabled = true;
                                existingEntity.UpdatedAt = DateTimeOffset.UtcNow;
                                if (evt.Version > existingEntity.Version)
                                {
                                    existingEntity.Version = evt.Version;
                                }
                                await _repository.UpdateAsync(existingEntity, cancellationToken);
                            }
                            else
                            {
                                var orphanEntity = new CmsEntity
                                {
                                    Id = evt.Id,
                                    Version = evt.Version,
                                    PayloadJson = "{}",
                                    IsDisabled = true,
                                    CreatedAt = DateTimeOffset.UtcNow,
                                    UpdatedAt = DateTimeOffset.UtcNow
                                };
                                await _repository.AddAsync(orphanEntity, cancellationToken);
                            }
                            break;

                        case "delete":
                            if (existingEntity != null)
                            {
                                await _repository.DeleteAsync(existingEntity, cancellationToken);
                            }
                            break;

                        default:
                            _logger.LogWarning("Unknown event type '{Type}' received for ID '{Id}'", evt.Type, evt.Id);
                            break;
                    }

                    await _repository.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Successfully processed event type '{Type}' for entity ID '{Id}'", evt.Type, evt.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process event type '{Type}' for entity ID '{Id}'", evt.Type, evt.Id);
                    throw;
                }
            }
        }
    }
}
