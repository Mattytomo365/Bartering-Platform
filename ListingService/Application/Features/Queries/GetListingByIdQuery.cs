using Application.DTOs;
using MediatR;

namespace Application.Features.Queries;

/// <summary>
/// Input model
/// Application query message to fetch a single Listing by Id
/// Carries data needed to perform the action
/// </summary>
public class GetListingByIdQuery : IRequest<ListingDetailDto>
{ 
    public Guid Id { get; init; } 
}
