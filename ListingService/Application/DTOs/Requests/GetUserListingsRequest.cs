namespace Application.DTOs.Requests;

// Request/input DTO - the shape of what the client sends
public class GetUserListingsRequest
{
    public required string OwnerId { get; init; }
}

