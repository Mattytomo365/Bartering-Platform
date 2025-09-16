namespace Application.DTOs.Requests;

/// <summary>
/// Request/input DTO - the shape of what the client sends for retrieving user listings
/// </summary>
public class GetUserListingsRequest
{
    public required string OwnerId { get; init; }
}

