namespace Domain.Events;

/// <summary>
/// Raised by Listing entity
/// Carries the full payload needed downstream for RabbitMQ publishing
/// Published by handlers
/// </summary>
public record ListingDeletedEvent(Guid ListingId);
