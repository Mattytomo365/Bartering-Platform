namespace Application.DTOs;

// Request DTO - the shape of what the client sends
public class UpdateListingRequest
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

