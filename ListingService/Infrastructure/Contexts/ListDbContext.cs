using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class ListDbContext : DbContext
{
    public ListDbContext(DbContextOptions<ListDbContext> opts) : base(opts) { }
    public virtual DbSet<Listing> Listings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Listing>(b => {
            b.HasKey(e => e.Id);
            b.Property(e => e.Title).HasMaxLength(200).IsRequired();
            b.Property(e => e.Description).IsRequired();
            b.OwnsOne(e => e.Price, m => {
                m.Property(p => p.Amount).HasColumnName("PriceAmount");
                m.Property(p => p.Currency).HasColumnName("PriceCurrency");
            });
            b.Property(e => e.Category).HasMaxLength(100).IsRequired();
            b.Property(e => e.Condition).HasMaxLength(50).IsRequired();
            b.Property(e => e.Latitude);
            b.Property(e => e.Longitude);
            b.Property(e => e.PhotoUrls).HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());
            b.Property(e => e.Wants).HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());
            b.Property(e => e.IsActive).HasDefaultValue(true);
            b.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
