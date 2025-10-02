using MediatR;

namespace Application.Features.Commands;

/// <summary>
/// Input model
/// Application message expressing the intent to project an incoming integration event into the read model
/// Carries data needed to perform the action
/// </summary>

public class UpsertListingCommand : IRequest<Unit>
{
    public Guid ListingId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }
    public required string Condition { get; init; }
    public DateTime CreatedAt { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
