using Application.Features.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscoveryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DiscoveryController(IMediator mediator) => _mediator = mediator;

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchListingsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Facets endpoint omitted for brevity; similar CQRS query can be added here
    }
}
