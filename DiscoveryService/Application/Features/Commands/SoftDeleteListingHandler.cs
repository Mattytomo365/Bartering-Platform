using Application.Interfaces;
using MediatR;

namespace Application.Features.Commands;

public class SoftDeleteListingHandler : IRequestHandler<SoftDeleteListingCommand, Unit>
{
    private readonly IListingRepository _repo;
    public SoftDeleteListingHandler(IListingRepository repo) => _repo = repo;

    public async Task<Unit> Handle(SoftDeleteListingCommand request, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(request.ListingId, ct);
        return Unit.Value;
    }
}
