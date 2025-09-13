using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ListingRepository : IListingRepository
{
    private readonly ListDbContext _db;
    public ListingRepository(ListDbContext db) { _db = db; }
    public async Task AddAsync(Listing listing) => await _db.Listings.AddAsync(listing);
    public async Task<Listing> GetByIdAsync(Guid id) =>
        await _db.Listings.FirstOrDefaultAsync(l => l.Id == id && l.IsActive)
        ?? throw new InvalidOperationException("Listing not found.");
    public async Task<IEnumerable<Listing>> GetByOwnerAsync(string ownerId) =>
        await _db.Listings.Where(l => l.OwnerId == ownerId && l.IsActive).ToListAsync();
    public Task UpdateAsync(Listing listing)
    {
        _db.Listings.Update(listing);
        return Task.CompletedTask;
    }
    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    public Task DeleteAsync(Listing listing)
    {
        _db.Listings.Remove(listing);
        return Task.CompletedTask;
    }
    
}
