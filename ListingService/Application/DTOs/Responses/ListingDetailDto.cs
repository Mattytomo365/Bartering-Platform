namespace Application.DTOs.Responses;

/// <summary>
/// Response/output DTO - read model returned to client after querying listing details
/// </summary>

public class ListingDetailDto
{
    public Guid Id { get; set; }
    public required string OwnerId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public required string Condition { get; set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public IEnumerable<string>? PhotoUrls { get; set; }
    public IEnumerable<string>? Wants { get; set; }
    public decimal PriceAmount { get; set; }
    public required string PriceCurrency { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
