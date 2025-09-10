using Application.DTOs;
using MediatR;

namespace Application.Features.Queries;

public class GetUserListingsQuery : IRequest<IEnumerable<ListingDto>> 
{ 
    public required string OwnerId { get; init; } 
}
