using Application.DTOs;
using MediatR;

namespace Application.Features.Queries;

/// <summary>  
/// A message capturing the intent of the user for reading listing state
/// </summary>
public class GetListingByIdQuery : IRequest<ListingDetailDto>
{ 
    public Guid Id { get; init; } 
}
