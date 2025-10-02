using Application.Features.Commands;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Handlers;

/// <summary>
/// Applies a listing.* event to the read model by upserting the SearchListing row
/// Idempotent by ListingId. No domain invariants here—this is a projection write use case
/// </summary>
public class UpsertListingHandler : IRequestHandler<UpsertListingCommand, Unit>
{
    private readonly IListingRepository _repo;
    public UpsertListingHandler(IListingRepository repo) => _repo = repo;

    public async Task<Unit> Handle(UpsertListingCommand request, CancellationToken ct)
    {
        await _repo.UpsertAsync(request, ct);
        return Unit.Value;
    }
}
