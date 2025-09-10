using MediatR;

namespace Application.Features.Commands;

public class DeleteListingCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}
