namespace Application.DTOs;

/// <summary>
/// Query DTO - read model returned to client corresponding to a search result row
/// </summary>
public class ListingDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double DistanceKm { get; set; }
}
