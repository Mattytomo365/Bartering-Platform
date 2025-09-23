using Application.Interfaces;
using MediatR;

namespace Application.Features.Commands;

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
