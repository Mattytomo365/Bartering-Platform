namespace Application.DTOs;

/// <summary>
/// Output model mapped from Listing aggregate (by handler) for a client-friendly output shape
/// </summary>
public class ListingDto
{
    public Guid Id { get; set; }
    public required string OwnerId { get; set; }
    public required string Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
