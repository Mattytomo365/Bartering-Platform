namespace Application.DTOs;

// Request DTO - input models shaping what clients send
public class GetListingByIdRequest
{
    public Guid Id { get; init; }
}

