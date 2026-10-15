using CmsIngestion.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CmsIngestion.Api.Controllers
{
    [ApiController]
    [Route("cms/entities")]
    public class EntitiesController : ControllerBase
    {
        private readonly ICmsEventService _eventService;

        public EntitiesController(ICmsEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEntities([FromHeader(Name = "X-User-Role")] string? userRole, CancellationToken cancellationToken)
        {
            bool isAdmin = string.Equals(userRole, "admin", StringComparison.OrdinalIgnoreCase);
            var entities = await _eventService.GetEntitiesForUserAsync(isAdmin, cancellationToken);
            return Ok(entities);
        }

        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> DisableEntity(string id, [FromHeader(Name = "X-User-Role")] string? userRole, CancellationToken cancellationToken)
        {
            if (!string.Equals(userRole, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Admin privileges required to disable entities." });
            }

            await _eventService.DisableEntityAsync(id, cancellationToken);
            return Ok(new { message = $"Entity '{id}' successfully disabled via admin override." });
        }
    }
}
