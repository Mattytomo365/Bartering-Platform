namespace Application.DTOs;

public class ListingDto
{
    public Guid Id { get; set; }
    public required string OwnerId { get; set; }
    public required string Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
