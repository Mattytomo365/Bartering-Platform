using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>  
/// Write-side repository
/// Persists and rehydrates Listing aggregate
/// </summary>

public class ProfileRepository : IProfileRepository
{
    private readonly ProfileDbContext _db;

    public ProfileRepository(ProfileDbContext db) => _db = db;

    // Write-side persistence
    public async Task UpsertAsync(ProfileLocation profile)
    {
        var existing = await _db.ProfileLocations.FirstOrDefaultAsync(p => p.UserId == profile.UserId);
        if (existing == null) await _db.ProfileLocations.AddAsync(profile);
        else
        {
            existing.DisplayName = profile.DisplayName;
            existing.Latitude = profile.Latitude;
            existing.Longitude = profile.Longitude;
        }
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    // Rehydration of aggregates, returned to handlers
    public async Task<ProfileLocation?> GetByUserIdAsync(string userId) =>
        await _db.ProfileLocations.FirstOrDefaultAsync(pl => pl.UserId == userId);


}
