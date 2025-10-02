using Application.Features.Commands;
using Application.Interfaces;
using Domain.ValueObjects;
using MediatR;
using Messaging.RabbitMQ;

namespace Application.Features.Handlers;

/// <summary>
/// Orchestrates and encapsulates write use case for domain modification, persistence, and event publication
/// Accepts UpdateListingCommand as input model
/// </summary>
public class UpdateListingHandler : IRequestHandler<UpdateListingCommand, Unit>
{
    private readonly IListingRepository _repo;
    private readonly IRabbitMqPublisher _publisher;

    public UpdateListingHandler(IListingRepository repo, IRabbitMqPublisher publisher)
    {
        _repo = repo;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(UpdateListingCommand req, CancellationToken ct)
    {
        var listing = await _repo.GetByIdAsync(req.Id); // repository rehydrates aggregate
        if (listing == null)
            throw new Exception($"Listing with ID {req.Id} not found.");

        var wants = req.Wants ?? new List<string>();

        listing.Update( // invariants enforced on changes made to aggregate
            req.Title, req.Description, new Money(req.PriceAmount, req.PriceCurrency), wants,
            req.Category, req.Condition, req.Latitude, req.Longitude
        );

 
        if (req.PhotoUrls != null && req.PhotoUrls.Count > 0) // PhotoUrls null validation
        {
            listing.PhotoUrls.Clear();
            listing.PhotoUrls.AddRange(req.PhotoUrls!);
        }

        await _repo.UpdateAsync(listing);
        await _repo.SaveChangesAsync();

        foreach (var @evt in listing.Events)
            await _publisher.PublishAsync(@evt);

        return Unit.Value;
    }
}
