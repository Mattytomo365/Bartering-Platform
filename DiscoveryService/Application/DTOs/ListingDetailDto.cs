namespace Application.DTOs;

public class ListingDetailDto

/// <summary>
/// Output model mapped from SearchListing entity for a client-friendly output shape
/// </summary>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double DistanceKm { get; set; }
}
