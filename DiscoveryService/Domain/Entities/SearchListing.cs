using NetTopologySuite.Geometries;

namespace Domain.Entities;

public class SearchListing
{
    public Guid ListingId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public required string Condition { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Available { get; set; }
    public required Point Location { get; set; }
}
