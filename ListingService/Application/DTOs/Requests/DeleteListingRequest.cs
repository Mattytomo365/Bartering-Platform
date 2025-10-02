namespace Application.DTOs.Requests;

/// <summary>
/// Input model
/// Transport shape carrying request data for listing deletion
/// </summary>
public class DeleteListingRequest
{
    public Guid Id { get; init; }
}

