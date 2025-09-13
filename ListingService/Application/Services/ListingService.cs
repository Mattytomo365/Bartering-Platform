using Application.DTOs;
using Application.Features.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using Domain.ValueObjects;
using MediatR;
using Messaging.RabbitMQ;

namespace Application.Services;

public class ListingService : IListingService
{
    private readonly IListingRepository _repo;
    private readonly IRabbitMqPublisher _publisher;

    public ListingService(IListingRepository repo, IRabbitMqPublisher publisher)
    {
        _repo = repo;
        _publisher = publisher;
    }

    public async Task<Guid> CreateListing(CreateListingRequest req, CancellationToken ct = default)
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

        listing.AddEvent(new ListingCreatedEvent(
            listing.Id, listing.Title, listing.Description, listing.Category, listing.Condition,
            listing.Latitude ?? 0, listing.Longitude ?? 0, listing.CreatedAt
        ));

        foreach (var @evt in listing.Events)
            await _publisher.PublishAsync(@evt);

        return listing.Id;
    }

    public async Task<Guid> UpdateListing(UpdateListingRequest req, CancellationToken ct = default)
    {
        var listing = await _repo.GetByIdAsync(req.Id);
        if (listing == null)
            throw new Exception($"Listing with ID {req.Id} not found.");

        // Ensure Wants is not null by creating a new list if it is null  
        var wants = req.Wants ?? new List<string>();

        listing.Update(
            req.Title, req.Description, new Money(req.PriceAmount, req.PriceCurrency), wants,
            req.Category, req.Condition, req.Latitude, req.Longitude
        );

        // Check PhotoUrls is not null  
        if (req.PhotoUrls != null && req.PhotoUrls.Count > 0)
        {
            listing.PhotoUrls.Clear();
            listing.PhotoUrls.AddRange(req.PhotoUrls!);
        }

        await _repo.UpdateAsync(listing);
        await _repo.SaveChangesAsync();

        foreach (var @evt in listing.Events)
            await _publisher.PublishAsync(@evt);

        return listing.Id;
    }

    public async Task<Guid> DeleteListing(Guid id, CancellationToken ct = default)
    {
        var listing = await _repo.GetByIdAsync(id);
        if (listing == null)
            throw new Exception($"Listing with ID {id} not found.");

        await _repo.DeleteAsync(listing);
        await _repo.SaveChangesAsync();

        listing.AddEvent(new ListingDeletedEvent(listing.Id));

        foreach (var @evt in listing.Events)
            await _publisher.PublishAsync(@evt);

        return listing.Id;
    }
}
