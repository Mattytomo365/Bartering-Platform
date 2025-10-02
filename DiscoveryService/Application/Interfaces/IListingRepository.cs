using Application.Features.Commands;
using Application.Features.Queries;
using Application.DTOs;

namespace Application.Interfaces;

public interface IListingRepository
{
    Task UpsertAsync(UpsertListingCommand command, CancellationToken ct);
    Task<SearchResultDto> SearchAsync(SearchListingsQuery query, CancellationToken ct);
    Task SoftDeleteAsync(Guid listingId, CancellationToken ct);
}