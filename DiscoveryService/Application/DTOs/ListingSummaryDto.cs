namespace Application.DTOs;

/// <summary>
/// Output model built from read model projection via repository for a client-friendly output shape
/// One projected SearchListing row
/// </summary>

public class ListingSummaryDto
{
    public Guid ListingId { get; set; }
    public required string Title { get; set; }
    public required string Category { get; set; }
    public required string Condition { get; set; }
    public required string Snippet { get; set; }
    public DateTime CreatedAt { get; set; }
    public double? DistanceKm { get; set; }
}
