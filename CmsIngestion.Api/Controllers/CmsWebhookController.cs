using CmsIngestion.Application.DTOs;
using CmsIngestion.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CmsIngestion.Api.Controllers
{
    [ApiController]
    [Route("cms/events")]
    public class CmsWebhookController : ControllerBase
    {
        private readonly ICmsEventService _eventService;

        public CmsWebhookController(ICmsEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost]
        public async Task<IActionResult> IngestEvents([FromBody] IEnumerable<CmsEventDto> events, CancellationToken cancellationToken)
        {
            if (events == null || !events.Any())
            {
                return BadRequest(new { error = "Event batch cannot be empty." });
            }

            await _eventService.ProcessEventsAsync(events, cancellationToken);
            return Ok(new { message = "Events processed successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents(CancellationToken cancellationToken)
        {
            var events = await _eventService.GetAllAsync(cancellationToken);
            return Ok(events);
        }
    }
}
