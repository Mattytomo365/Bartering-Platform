using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Application.DTOs;
using Application.Features.Queries;
using Application.Features.Commands;

namespace Infrastructure.Repositories;

// repository on read-side

/// <summary>
/// Repository for managing <see cref="SearchListing"/> entities, including upsert, search, and soft delete operations.
/// </summary>
public class ListingRepository : IListingRepository
{
    private readonly SearchDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListingRepository"/> class.
    /// </summary>
    /// <param name="db">The database context to use for data access.</param>
    public ListingRepository(SearchDbContext db) => _db = db;

    /// <summary>
    /// Inserts or updates a <see cref="SearchListing"/> entity based on the provided command.
    /// </summary>
    /// <param name="command">The upsert command containing listing data.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpsertAsync(UpsertListingCommand command, CancellationToken ct)
    {
        var existing = await _db.Listings.FindAsync(new object[] { command.ListingId }, ct);
        if (existing == null)
        {
            existing = new SearchListing
            {
                ListingId = command.ListingId,
                Title = command.Title,
                Description = command.Description,
                Category = command.Category,
                Condition = command.Condition,
                Location = new Point(command.Longitude, command.Latitude) { SRID = 4326 },
                CreatedAt = command.CreatedAt,
                Available = true
            };
            _db.Listings.Add(existing);
        }

        existing.Title = command.Title;
        existing.Description = command.Description;
        existing.Category = command.Category;
        existing.Condition = command.Condition;
        existing.CreatedAt = command.CreatedAt;
        existing.Available = true;
        existing.Location = new Point(command.Longitude, command.Latitude) { SRID = 4326 };

        await _db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Searches for listings based on the specified query parameters.
    /// Supports full-text search, category filtering, location-based filtering, and sorting.
    /// </summary>
    /// <param name="req">The search query parameters.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A <see cref="SearchResult"/> containing the search results and metadata.</returns>
    /// <exception cref="InvalidOperationException">Thrown if full-text search is requested but not configured.</exception>
    public async Task<SearchResult> SearchAsync(SearchListingsQuery req, CancellationToken ct)
    {
        // Initialize user location point if latitude and longitude are provided
        Point? userPoint = null;
        if (req.Lat.HasValue && req.Lng.HasValue)
            userPoint = new Point(req.Lng.Value, req.Lat.Value) { SRID = 4326 };

        // If a full-text search query is provided
        if (!string.IsNullOrWhiteSpace(req.Q))
        {
            // Ensure the FullTextMatches DbSet is configured
            if (_db.FullTextMatches == null)
                throw new InvalidOperationException("FullTextMatches DbSet is not configured in the SearchDbContext.");

            // Query the FREETEXTTABLE for matches on Title and Description
            var matches = _db.FullTextMatches
                .FromSqlInterpolated($@"
                    SELECT [KEY], [RANK]
                    FROM FREETEXTTABLE(
                          dbo.Listings, 
                          (Title, Description), 
                          {req.Q}
                    )");

            // Join listings with full-text matches and filter for available listings
            var ftQuery = from l in _db.Listings.AsNoTracking().Where(l => l.Available)
                          join m in matches on l.ListingId equals m.ListingId
                          select new { Listing = l, m.Rank };

            // Filter by category if specified (case-insensitive, skip if "All")
            if (!string.IsNullOrWhiteSpace(req.Category) && !string.Equals(req.Category, "All", StringComparison.OrdinalIgnoreCase))
                ftQuery = ftQuery.Where(x => x.Listing.Category.ToLower() == req.Category.ToLower());

            // Filter by distance if user location is provided
            if (userPoint != null)
                ftQuery = ftQuery.Where(x =>
                    x.Listing.Location.Distance(userPoint) <= req.RadiusKm * 1000
                );

            // Apply sorting based on the requested sort order
            ftQuery = req.Sort switch
            {
                "newest" => ftQuery.OrderByDescending(x => x.Listing.CreatedAt),
                "distance" when userPoint != null
                           => ftQuery.OrderBy(x => x.Listing.Location.Distance(userPoint)),
                _ => ftQuery.OrderByDescending(x => x.Rank)
            };

            // Execute count and paged query sequentially to avoid DbContext concurrency issues
            var total = await ftQuery.CountAsync(ct);
            var items = await ftQuery
                .Skip((req.Page!.Value - 1) * req.PageSize!.Value)
                .Take(req.PageSize!.Value)
                .Select(x => new ListingSummary
                {
                    ListingId = x.Listing.ListingId,
                    Title = x.Listing.Title,
                    Category = x.Listing.Category,
                    Condition = x.Listing.Condition,
                    // Truncate description to 200 characters for snippet
                    Snippet = x.Listing.Description.Length > 200
                                    ? x.Listing.Description.Substring(0, 200)
                                    : x.Listing.Description,
                    CreatedAt = x.Listing.CreatedAt,
                    // Calculate distance in kilometers if user location is provided
                    DistanceKm = userPoint != null
                                 ? x.Listing.Location.Distance(userPoint) / 1000
                                 : (double?)null
                })
                .ToListAsync(ct);

            // Return the search result with total count and paged items
            return new SearchResult
            {
                Total = total,
                Page = req.Page.Value,
                PageSize = req.PageSize.Value,
                Results = items
            };
        }
        else
        {
            // Query for available listings
            var listings = _db.Listings
                              .AsNoTracking()
                              .Where(l => l.Available);

            // Filter by category if specified (case-insensitive, skip if "All")
            if (!string.IsNullOrWhiteSpace(req.Category) && !string.Equals(req.Category, "All", StringComparison.OrdinalIgnoreCase))
                listings = listings.Where(l => l.Category.ToLower() == req.Category.ToLower());

            // Filter by distance if user location is provided
            if (userPoint != null)
                listings = listings.Where(l =>
                    l.Location.Distance(userPoint) <= req.RadiusKm * 1000
                );

            // Apply sorting based on the requested sort order
            listings = req.Sort switch
            {
                "newest" => listings.OrderByDescending(l => l.CreatedAt),
                "distance" when userPoint != null
                           => listings.OrderBy(l => l.Location.Distance(userPoint)),
                _ => listings
            };

            // Execute count and paged query sequentially to avoid DbContext concurrency issues
            var total = await listings.CountAsync(ct);
            var items = await listings
                .Skip((req.Page!.Value - 1) * req.PageSize!.Value)
                .Take(req.PageSize!.Value)
                .Select(l => new ListingSummary
                {
                    ListingId = l.ListingId,
                    Title = l.Title,
                    Category = l.Category,
                    Condition = l.Condition,
                    // Truncate description to 200 characters for snippet
                    Snippet = l.Description.Length > 200
                                    ? l.Description.Substring(0, 200)
                                    : l.Description,
                    CreatedAt = l.CreatedAt,
                    // Calculate distance in kilometers if user location is provided
                    DistanceKm = userPoint != null
                                 ? l.Location.Distance(userPoint) / 1000
                                 : (double?)null
                })
                .ToListAsync(ct);

            // Return the search result with total count and paged items
            return new SearchResult
            {
                Total = total,
                Page = req.Page.Value,
                PageSize = req.PageSize.Value,
                Results = items
            };
        }
    }

    /// <summary>
    /// Soft deletes a listing by marking it as unavailable.
    /// </summary>
    /// <param name="listingId">The unique identifier of the listing to delete.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SoftDeleteAsync(Guid listingId, CancellationToken ct)
    {
        var existing = await _db.Listings.FindAsync(new object[] { listingId }, ct);
        if (existing != null)
        {
            existing.Available = false;
            await _db.SaveChangesAsync(ct);
        }
    }
}