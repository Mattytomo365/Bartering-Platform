using Domain.Entities;
using Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class SearchDbContext(DbContextOptions<SearchDbContext> options) : DbContext(options)
{
    public virtual DbSet<SearchListing> Listings => Set<SearchListing>();
    public DbSet<FullTextMatch>? FullTextMatches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SearchListing>()
            .HasKey(l => l.ListingId)
            .HasName("PK_Listings");

        // Spatial configuration
        modelBuilder.Entity<SearchListing>()
            .Property(l => l.Location)
            .HasColumnType("geography");

        // No backing table/view – this is just for FromSql
        modelBuilder.Entity<FullTextMatch>()
            .HasNoKey()
            .ToView(null);

        base.OnModelCreating(modelBuilder);
    }
}
