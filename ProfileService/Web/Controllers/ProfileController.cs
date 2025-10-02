using Application.DTOs;
using Application.Features.Commands;
using Application.Features.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

/// <summary>
/// HTTP API for user profile data (e.g., current location).
/// Provides a write endpoint to upsert the caller's location and a read endpoint
/// to retrieve it. The database here is the write model (source of truth);
/// no separate projection is used. The current user identity (UserId) is resolved
/// server-side (e.g., from JWT) rather than accepted from the client.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProfileController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets location corresponding to user profile
    /// Returns 200 OK with ProfileLocationDto output shape
    /// </summary>
    [HttpGet("location")]
    public async Task<ActionResult<ProfileLocationDto>> GetLocation()
    {
        var loc = await _mediator.Send(new GetProfileLocationQuery());
        if (loc == null) return NotFound();
        return Ok(loc);
    }

    /// <summary>
    /// Creates or updates the location information for a user profile
    /// Returns 204 No Content on success 
    /// </summary>
    [HttpPost("location")]
    public async Task<IActionResult> UpsertLocation(UpsertProfileLocationCommand cmd, CancellationToken ct)
    {
        await _mediator.Send(cmd, ct);
        return NoContent();
    }
}
