using Application.DTOs;
using Application.Features.Commands;
using Application.Features.Queries;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ListingsController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new listing
        /// Returns 201 Created with a Location header pointing to GET /api/listings/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateListingCommand cmd)
        {
            var id = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        /// <summary>
        /// Fully updates an existing listing (id from route, fields from body)
        /// Returns 204 No Content on success
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateListingCommand cmd)
        {
            if (id != cmd.Id) return BadRequest();
            await _mediator.Send(cmd);
            return NoContent();
        }

        /// <summary>
        /// Hard deletes a listing by id
        /// Returns 204 No Content
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteListingCommand { Id = id });
            return NoContent();
        }

        /// <summary>
        /// Gets a single listing with detail fields suitable for a detail page
        /// Returns 200 OK with ListingDetailDto
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dto = await _mediator.Send(new GetListingByIdQuery { Id = id });
            return Ok(dto);
        }

        /// <summary>
        /// Gets all active listings for an owner (lightweight cards/table)
        /// Returns 200 OK with a collection of ListingDto
        /// </summary
        [HttpGet("user/{ownerId}")]
        public async Task<IActionResult> GetUserListings(string ownerId)
        {
            var dtos = await _mediator.Send(new GetUserListingsQuery { OwnerId = ownerId });
            return Ok(dtos);
        }
    }
}
