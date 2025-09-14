namespace Application.DTOs;

// Request DTO - the shape of what the client sends
public class GetUserListingsRequest
{
    public required string OwnerId { get; init; }
}

