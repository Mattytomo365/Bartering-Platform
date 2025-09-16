namespace Application.DTOs.Requests;

/// <summary>
/// Request/input DTO - the shape of what the client sends for reading listings by id
/// </summary>
public class GetListingByIdRequest
{
    public Guid Id { get; init; }
}

