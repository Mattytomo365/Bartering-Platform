using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class ProfileDbContext : DbContext
{
    public ProfileDbContext(DbContextOptions<ProfileDbContext> opts) : base(opts) { }

    public virtual DbSet<ProfileLocation> ProfileLocations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ProfileLocation>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.UserId).IsRequired();
            b.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            b.Property(e => e.Latitude).IsRequired();
            b.Property(e => e.Longitude).IsRequired();
        });
    }
}
