namespace Application.DTOs.Requests;

/// <summary>
/// Request/input DTO - the shape of what the client sends for deleting listings
/// </summary>
public class DeleteListingRequest
{
    public Guid Id { get; init; }
}

