using Application.DTOs.Requests;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly IListingService _listingService;

        public ListingsController(IListingService listingService) 
        {
            _listingService = listingService;
        }

        /// <summary>
        /// Creates a new listing
        /// Returns 201 Created with a Location header pointing to GET /api/listings/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateListingRequest req)
        {
            var id = await _listingService.CreateListing(req);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        /// <summary>
        /// Fully updates an existing listing (id from route, fields from body)
        /// Returns 204 No Content on success
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateListingRequest req)
        {
            if (id != req.Id) return BadRequest(); // guard against mismatched ids
            await _listingService.UpdateListing(req);
            return NoContent();
        }

        /// <summary>
        /// Hard deletes a listing by id
        /// Returns 204 No Content
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _listingService.DeleteListing(id, ct);
            return NoContent();
        }

        /// <summary>
        /// Gets a single listing with detail fields suitable for a detail page
        /// Returns 200 OK with ListingDetailDto
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _listingService.GetListingById(new GetListingByIdRequest { Id = id }, ct);
            return Ok(dto);
        }

        /// <summary>
        /// Gets all active listings for an owner (lightweight cards/table)
        /// Returns 200 OK with a collection of ListingDto
        /// </summary
        [HttpGet("user/{ownerId}")]
        public async Task<IActionResult> GetUserListings(string ownerId, CancellationToken ct)
        {
            var dtos = await _listingService.GetUserListings(new GetUserListingsRequest { OwnerId = ownerId }, ct);
            return Ok(dtos);
        }
    }
}
