using Application.Features.Commands;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Handlers;

/// <summary>
/// Applies command to the write model by upserting location via constructed ProfileLocation aggregate to enforce invariants
/// Persists via repository
/// </summary>

public class UpsertProfileLocationHandler : IRequestHandler<UpsertProfileLocationCommand, Unit>
{
    private readonly IProfileRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public UpsertProfileLocationHandler(IProfileRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpsertProfileLocationCommand req, CancellationToken ct)
    {
        var profile = new ProfileLocation
        {
            UserId = _currentUser.UserId,
            DisplayName = req.Location.DisplayName,
            Latitude = req.Location.Latitude,
            Longitude = req.Location.Longitude
        };

        await _repo.UpsertAsync(profile);
        await _repo.SaveChangesAsync();

        return Unit.Value;
    }
}
