using Application.DTOs;
using MediatR;

namespace Application.Features.Queries;
/// <summary>  
/// A message capturing the intent of the user for reading user listings state
/// </summary>

public class GetUserListingsQuery : IRequest<IEnumerable<ListingDto>> 
{ 
    public required string OwnerId { get; init; } 
}
