namespace Domain.Entities;

public class ProfileLocation
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}