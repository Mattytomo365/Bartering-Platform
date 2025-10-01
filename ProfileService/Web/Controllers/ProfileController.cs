using Application.DTOs;
using Application.Features.Commands;
using Application.Features.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProfileController(IMediator mediator) => _mediator = mediator;

    [HttpGet("location")]
    public async Task<ActionResult<ProfileLocationDto>> GetLocation()
    {
        var loc = await _mediator.Send(new GetProfileLocationQuery());
        if (loc == null) return NotFound();
        return Ok(loc);
    }

    [HttpPost("location")]
    public async Task<IActionResult> UpsertLocation(UpsertProfileLocationCommand cmd, CancellationToken ct)
    {
        await _mediator.Send(cmd, ct);
        return NoContent();
    }
}
