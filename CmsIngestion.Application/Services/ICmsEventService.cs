using CmsIngestion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsIngestion.Application.Services
{
    public interface ICmsEventService
    {
        Task ProcessEventsAsync(IEnumerable<CmsEventDto> events, CancellationToken cancellationToken = default);
    }
}
