using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ProfileDbContext _db;

    public ProfileRepository(ProfileDbContext db) => _db = db;

    public async Task<ProfileLocation?> GetByUserIdAsync(string userId) =>
        await _db.ProfileLocations.FirstOrDefaultAsync(pl => pl.UserId == userId);

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
}
