using MediatR;

namespace Application.Features.Commands;

/// <summary>  
/// A message capturing the intent of the user for changing listing state
/// </summary>
public class DeleteListingCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}
