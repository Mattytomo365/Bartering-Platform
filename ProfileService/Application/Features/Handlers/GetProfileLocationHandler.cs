using Application.DTOs;
using Application.Features.Queries;
using Application.Interfaces;
using MediatR;

/// <summary>
/// Fetches single listing by Id from write model, read use case
/// Accepts GetProfileLocation as input model
/// Maps returned aggregate to a detail DTO output shape
/// </summary>
namespace Application.Features.Handlers;
public class GetProfileLocationHandler : IRequestHandler<GetProfileLocationQuery, ProfileLocationDto?>
{
    private readonly IProfileRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public GetProfileLocationHandler(IProfileRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<ProfileLocationDto?> Handle(GetProfileLocationQuery req, CancellationToken ct)
    {
        var profile = await _repo.GetByUserIdAsync(_currentUser.UserId);
        if (profile == null) return null;

        return new ProfileLocationDto
        {
            DisplayName = profile.DisplayName,
            Latitude = profile.Latitude,
            Longitude = profile.Longitude
        };
    }
}
