namespace Domain.Events;

/// <summary>
/// Carries the full payload needed downstream
/// </summary>
public record ListingUpdatedEvent(
    Guid ListingId,
    string Title,
    string Description,
    string Category,
    string Condition,
    double Latitude,
    double Longitude,
    DateTime CreatedAt
);
