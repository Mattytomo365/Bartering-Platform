using Application.Interfaces;
using MediatR;
using Messaging.RabbitMQ;

namespace Application.Features.Commands;

/// <summary>
/// Implements the logic for executing the DeleteListing request
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
        //listing.Delete();

        await _repo.UpdateAsync(listing);
        await _repo.SaveChangesAsync();

        foreach (var @evt in listing.Events)
            await _publisher.PublishAsync(@evt);

        return Unit.Value;
    }
}
