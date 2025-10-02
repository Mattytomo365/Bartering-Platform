using Application.DTOs;
using MediatR;

namespace Application.Features.Queries;

/// <summary>
/// Input model
/// Application query message to fetch listings related to a specific user
/// Carries data needed to perform the action
/// </summary>

public class GetUserListingsQuery : IRequest<IEnumerable<ListingDto>> 
{ 
    public required string OwnerId { get; init; } 
}
