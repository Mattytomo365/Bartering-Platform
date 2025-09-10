using MediatR;

namespace Application.Features.Commands;

public class SoftDeleteListingCommand : IRequest<Unit>
{
    public Guid ListingId { get; init; }
}
