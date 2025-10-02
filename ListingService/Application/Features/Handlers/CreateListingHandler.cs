using Application.Features.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Messaging.RabbitMQ;

namespace Application.Features.Handlers;

/// <summary>
/// Orchestrates and encapsulates write use case for domain creation, persistence, and event publication
/// Accepts CreateListingCommand as input model
/// Constructs aggregate and enforces invariants before persisting
/// </summary>

public class CreateListingHandler : IRequestHandler<CreateListingCommand, Guid>
{
    private readonly IListingRepository _repo;
    private readonly IRabbitMqPublisher _publisher;

    public CreateListingHandler(IListingRepository repo, IRabbitMqPublisher publisher)
    {
        _repo = repo;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(CreateListingCommand req, CancellationToken ct)
    {
        // Enforces invariants using constructor
        var listing = new Listing(
            ownerId: req.OwnerId,
            title: req.Title,
            description: req.Description,
            price: new Money(req.PriceAmount, req.PriceCurrency),
            wants: req.Wants ?? new List<string>(),
            category: req.Category,
            condition: req.Condition,
            latitude: req.Latitude,
            longitude: req.Longitude
        );

        // Populates photos (and any other collections)
        if (req.PhotoUrls != null && req.PhotoUrls.Count > 0)
        {
            listing.PhotoUrls.AddRange(req.PhotoUrls);
        }

        await _repo.AddAsync(listing);
        await _repo.SaveChangesAsync();

        foreach (var @evt in listing.Events)
            await _publisher.PublishAsync(@evt);

        return listing.Id;
    }
}
