namespace Domain.Events;

/// <summary>
/// Raised by Listing entity
/// Carries the full payload needed downstream for RabbitMQ publishing
/// Published by handlers
/// </summary>
public record ListingCreatedEvent(
    Guid ListingId,
    string Title,
    string Description,
    string Category,
    string Condition,
    double Latitude,
    double Longitude,
    DateTime CreatedAt
);
