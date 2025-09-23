using Application.DTOs;
using Application.Features.Queries;
using Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Features.Handlers;
/// <summary>
/// Implements the logic for executing the GetListingById request
/// </summary>

public class GetListingByIdHandler : IRequestHandler<GetListingByIdQuery, ListingDetailDto>
{
    private readonly IListingRepository _repo;
    private readonly IMapper _mapper;
    public GetListingByIdHandler(IListingRepository repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
    public async Task<ListingDetailDto> Handle(GetListingByIdQuery req, CancellationToken ct)
    {
        var listing = await _repo.GetByIdAsync(req.Id);
        return _mapper.Map<ListingDetailDto>(listing);
    }
}
