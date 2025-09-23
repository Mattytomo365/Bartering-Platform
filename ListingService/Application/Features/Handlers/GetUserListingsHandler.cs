using Application.DTOs;
using Application.Features.Queries;
using Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Features.Handlers;

/// <summary>  
/// Implements the logic for executing the GetUserListingsHandler request
/// </summary>  
public class GetUserListingsHandler : IRequestHandler<GetUserListingsQuery, IEnumerable<ListingDto>>
{
    private readonly IListingRepository _repo;
    private readonly IMapper _mapper;

    /// <summary>  
    /// Initializes a new instance of the <see cref="GetUserListingsHandler"/> class.  
    /// </summary>  
    /// <param name="repo">The repository for accessing listing data.</param>  
    /// <param name="mapper">The mapper for converting entities to DTOs.</param>  
    public GetUserListingsHandler(IListingRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    /// <summary>  
    /// Handles the query to retrieve listings for a specific user.  
    /// </summary>  
    /// <param name="req">The query containing the owner ID.</param>  
    /// <param name="ct">The cancellation token.</param>  
    /// <returns>A collection of <see cref="ListingDto"/> objects representing the user's listings.</returns>  
    public async Task<IEnumerable<ListingDto>> Handle(GetUserListingsQuery req, CancellationToken ct)
    {
        var listings = await _repo.GetByOwnerAsync(req.OwnerId);
        return _mapper.Map<IEnumerable<ListingDto>>(listings);
    }
}
