namespace Domain.Entities;

/// <summary>
/// Domain entity / write model / aggregate root for profile actions
/// Encapsulates state + behavior and enforces few invariants
/// </summary>

public class ProfileLocation
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}