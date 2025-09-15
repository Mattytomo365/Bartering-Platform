namespace Application.DTOs.Requests;

// Request/input DTO - the shape of what the client sends
public class GetListingByIdRequest
{
    public Guid Id { get; init; }
}

