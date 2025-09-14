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
        private readonly IListingService _listingService;

        public ListingsController(IMediator mediator,
                                  IListingService listingService) 
        {
            _mediator = mediator;
            _listingService = listingService;
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateListingCommand cmd)
        //{
        //    var id = await _mediator.Send(cmd);
        //    return CreatedAtAction(nameof(GetById), new { id }, null);
        //}

        [HttpPost]
        public async Task<IActionResult> Create(CreateListingRequest req)
        {
            var id = await _listingService.CreateListing(req);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(Guid id, UpdateListingCommand cmd)
        //{
        //    if (id != cmd.Id) return BadRequest();
        //    await _mediator.Send(cmd);
        //    return NoContent();
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateListingRequest req)
        {
            if (id != req.Id) return BadRequest();
            await _listingService.UpdateListing(req);
            return NoContent();
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    await _mediator.Send(new DeleteListingCommand { Id = id });
        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _listingService.DeleteListing(id, ct);
            return NoContent();
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    var dto = await _mediator.Send(new GetListingByIdQuery { Id = id });
        //    return Ok(dto);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _listingService.GetListingById(new GetListingByIdRequest { Id = id }, ct);
            return Ok(dto);
        }

        //[HttpGet("user/{ownerId}")]
        //public async Task<IActionResult> GetUserListings(string ownerId)
        //{
        //    var dtos = await _mediator.Send(new GetUserListingsQuery { OwnerId = ownerId });
        //    return Ok(dtos);
        //}

        [HttpGet("user/{ownerId}")]
        public async Task<IActionResult> GetUserListings(string ownerId, CancellationToken ct)
        {
            var dtos = await _listingService.GetUserListings(new GetUserListingsRequest { OwnerId = ownerId }, ct);
            return Ok(dtos);
        }
    }
}
