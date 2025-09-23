using Application.Features.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Messaging.RabbitMQ;

namespace Application.Features.Handlers;

/// <summary>
/// Implements the logic for executing the CreateListing request
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
        // 1) Use the constructor that enforces invariants
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

        // 2) Now populate the photos (and any other collections)
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
