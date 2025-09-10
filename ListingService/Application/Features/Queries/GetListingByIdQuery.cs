using Application.DTOs;
using MediatR;

namespace Application.Features.Queries;

public class GetListingByIdQuery : IRequest<ListingDetailDto>
{ 
    public Guid Id { get; init; } 
}
