namespace Application.DTOs;

public class ListingSummary
{
    public Guid ListingId { get; set; }
    public required string Title { get; set; }
    public required string Category { get; set; }
    public required string Condition { get; set; }
    public required string Snippet { get; set; }
    public DateTime CreatedAt { get; set; }
    public double? DistanceKm { get; set; }
}
