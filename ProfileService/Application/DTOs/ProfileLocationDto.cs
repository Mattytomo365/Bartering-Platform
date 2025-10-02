namespace Application.DTOs;

/// <summary>
/// Output model mapped from ProfileLocation aggregate (by handler) for a client-friendly output shape
/// </summary>

public class ProfileLocationDto
{
    public string DisplayName { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
