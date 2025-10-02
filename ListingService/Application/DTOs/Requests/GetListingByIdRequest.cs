namespace Application.DTOs.Requests;

/// <summary>
/// Input model
/// Transport shape carrying request data for listing by id retrieval
/// </summary>
public class GetListingByIdRequest
{
    public Guid Id { get; init; }
}

