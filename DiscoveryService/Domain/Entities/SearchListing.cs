using NetTopologySuite.Geometries;

namespace Domain.Entities;

/// <summary>
/// Read-model projection used by DiscoveryService for search/FTS.
/// Not a domain aggregate: no business invariants or domain events.
/// Populated asynchronously from listing.* events.
/// </summary>

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
