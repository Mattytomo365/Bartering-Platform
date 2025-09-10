using Application.Interfaces;
using Domain.ValueObjects;
using MediatR;
using Messaging.RabbitMQ;

namespace Application.Features.Commands;

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

        return Unit.Value;
    }
}
