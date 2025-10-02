using Application.Features.Commands;
using Application.Interfaces;
using MediatR;
using Messaging.RabbitMQ;

namespace Application.Features.Handlers;

/// <summary>
/// Orchestrates and encapsulates write use case for domain deletion, persistence, and event publication
/// Accepts DeleteListingCommand as input model
/// </summary>

public class DeleteListingHandler : IRequestHandler<DeleteListingCommand, Unit>
{
    private readonly IListingRepository _repo;
    private readonly IRabbitMqPublisher _publisher;
    public DeleteListingHandler(IListingRepository repo, IRabbitMqPublisher publisher)
    {
        _repo = repo; _publisher = publisher;
    }
    public async Task<Unit> Handle(DeleteListingCommand req, CancellationToken ct)
    {
        var listing = await _repo.GetByIdAsync(req.Id);

        await _repo.UpdateAsync(listing);
        await _repo.SaveChangesAsync();

        foreach (var @evt in listing.Events)
            await _publisher.PublishAsync(@evt);

        return Unit.Value;
    }
}
