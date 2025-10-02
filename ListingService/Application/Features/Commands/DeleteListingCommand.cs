using MediatR;

namespace Application.Features.Commands;

/// <summary>
/// Input model
/// A message expressing the intent for deleting a listing
/// Carries data needed to perform the action
/// </summary>
public class DeleteListingCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}
