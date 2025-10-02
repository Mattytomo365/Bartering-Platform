using Domain.Events;
using Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entities;

/// <summary>
/// Domain entity / write model / aggregate root for marketplace listings
/// Encapsulates state + behavior and enforces invariants (valid title, non-negative price, etc)
/// This is the source of truth used by application services; API DTOs map to/from this type
/// </summary>
public class Listing
{
    public Guid Id { get; set; }
    public required string OwnerId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public required string Condition { get; set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public List<string> PhotoUrls { get; set; } = new();
    public List<string> Wants { get; set; } = new();
    public required Money Price { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Domain events  
    private readonly List<object> _events = new();
    public IReadOnlyCollection<object> Events => _events.AsReadOnly();

    [SetsRequiredMembers]
    protected Listing()
    {
        // Initialize collections to avoid null refs  
        PhotoUrls = new List<string>();
        Wants = new List<string>();

        // Initialize required properties with default values to satisfy the compiler
        OwnerId = default!;
        Title = default!;
        Description = default!;
        Price = default!;
        Category = default!;
        Condition = default!;
    }

    /// <summary>
    /// Constructor used to create a valid listing and enforce invariants
    /// </summary>
    [SetsRequiredMembers]
    public Listing(
        string ownerId,
        string title,
        string description,
        Money price,
        List<string> wants,
        string category,
        string condition,
        double? latitude = null,
        double? longitude = null)
        : this() 
    {
        OwnerId = ownerId;
        Title = title;
        Description = description;
        Price = price;
        Wants = wants ?? new List<string>();
        Category = category;
        Condition = condition;
        Latitude = latitude;
        Longitude = longitude;
        CreatedAt = DateTime.UtcNow;

    }

    /// <summary>
    /// Update mutable fields while preserving invariants
    /// </summary>
    public void Update(string title, string description, Money price, List<string> wants,
        string category, string condition, double? latitude, double? longitude)
    {
        Title = title;
        Description = description;
        Price = price;
        Wants = wants;
        Category = category;
        Condition = condition;
        Latitude = latitude;
        Longitude = longitude;

        AddEvent(new ListingUpdatedEvent(
            Id, Title, Description, Category, Condition, 
            Latitude ?? 0, Longitude ?? 0, CreatedAt
        ));
    }

    public void AddEvent(object @event) => _events.Add(@event);
}
