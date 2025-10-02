using Application.DTOs;
using Application.Features.Queries;
using Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Features.Handlers;

/// <summary>
/// Fetches single listings related to a specific user, read use case 
/// Accepts GetUserListingsQuery as input model
/// Maps returned aggregate to a listing DTO output shape
/// </summary>  
public class GetUserListingsHandler : IRequestHandler<GetUserListingsQuery, IEnumerable<ListingDto>>
{
    private readonly IListingRepository _repo;
    private readonly IMapper _mapper;
 
    public GetUserListingsHandler(IListingRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ListingDto>> Handle(GetUserListingsQuery req, CancellationToken ct)
    {
        var listings = await _repo.GetByOwnerAsync(req.OwnerId);
        return _mapper.Map<IEnumerable<ListingDto>>(listings);
    }
}
