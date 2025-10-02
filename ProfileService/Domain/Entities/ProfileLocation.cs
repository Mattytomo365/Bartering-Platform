namespace Domain.Entities;

/// <summary>
/// Domain entity / write model / aggregate root for profile locations
/// Encapsulates state + behavior 
/// This is the source of truth used by application services; API DTOs map to/from this type
/// </summary>

public class ProfileLocation
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}