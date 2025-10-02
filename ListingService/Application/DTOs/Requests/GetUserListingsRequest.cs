namespace Application.DTOs.Requests;

/// <summary>
/// Input model
/// Transport shape carrying request data for user listing retrieval
/// </summary>
public class GetUserListingsRequest
{
    public required string OwnerId { get; init; }
}

