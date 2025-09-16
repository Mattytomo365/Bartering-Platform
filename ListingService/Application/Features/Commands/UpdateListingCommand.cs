using MediatR;

namespace Application.Features.Commands;

/// <summary>  
/// A message capturing the intent of the user for changing listing state
/// </summary>
public class UpdateListingCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public decimal PriceAmount { get; init; }
    public required string PriceCurrency { get; init; }
    public List<string>? Wants { get; init; }
    public List<string>? PhotoUrls { get; init; }
    public required string Category { get; init; }
    public required string Condition { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
