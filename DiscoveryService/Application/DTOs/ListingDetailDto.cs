namespace Application.DTOs;

public class ListingDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double DistanceKm { get; set; }
}
