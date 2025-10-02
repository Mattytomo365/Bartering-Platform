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


        /// <summary>
        /// Executes a listings search over the Discovery read model (SQL Server FTS + filters).
        /// Supports text (q), category, optional geo radius, sorting, and paging.
        /// Returns a <see cref="SearchResultDto"/> envelope containing <see cref="ListingSummaryDto"/> items.
        /// </summary>

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchListingsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
