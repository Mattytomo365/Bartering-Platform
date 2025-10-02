using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>  
/// Write-side repository
/// Persists and rehydrates Listing aggregate
/// </summary>

public class ListingRepository : IListingRepository
{
    private readonly ListDbContext _db;
    public ListingRepository(ListDbContext db) { _db = db; }

    // Write-side persistence
    public async Task AddAsync(Listing listing) => await _db.Listings.AddAsync(listing);
    public Task UpdateAsync(Listing listing)
    {
        _db.Listings.Update(listing);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(Listing listing)
    {
        _db.Listings.Remove(listing);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    // Rehydraton & simple non-search reads from write DB, returns aggregates to be mapped within handlers
    public async Task<Listing> GetByIdAsync(Guid id) =>
        await _db.Listings.FirstOrDefaultAsync(l => l.Id == id && l.IsActive)
        ?? throw new InvalidOperationException("Listing not found.");
    public async Task<IEnumerable<Listing>> GetByOwnerAsync(string ownerId) =>
        await _db.Listings.Where(l => l.OwnerId == ownerId && l.IsActive).ToListAsync();

    


    
}
