using MediatR;

namespace Application.Features.Commands;

/// <summary>
/// Input model
/// Application message expressing the intent to remove a listing from the read model
/// Carries data needed to perform the action
/// </summary>

public class SoftDeleteListingCommand : IRequest<Unit>
{
    public Guid ListingId { get; init; }
}
