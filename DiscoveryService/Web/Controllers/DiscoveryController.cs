using Application.Features.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers

/// <summary>
/// Read-only API over the Discovery read model (projection).
/// Exposes search endpoints backed by SQL Server Full-Text Search (FTS),
/// with optional category/geo filters, sorting, and paging.
/// This controller does not mutate state; projection writes are performed
/// asynchronously by the RabbitMQ consumer applying listing.* events.
/// </summary>
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
