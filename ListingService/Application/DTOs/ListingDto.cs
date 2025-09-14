namespace Application.DTOs;

// Response DTO - output models shaping what is returned
public class ListingDto
{
    public Guid Id { get; set; }
    public required string OwnerId { get; set; }
    public required string Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
