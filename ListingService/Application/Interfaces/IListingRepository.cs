using Domain.Entities;

namespace Application.Interfaces;

public interface IListingRepository
{
    Task AddAsync(Listing listing);
    Task<Listing> GetByIdAsync(Guid id);
    Task<IEnumerable<Listing>> GetByOwnerAsync(string ownerId);
    Task UpdateAsync(Listing listing);
    Task SaveChangesAsync();
}
