using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProfileRepository
    {
        Task<ProfileLocation?> GetByUserIdAsync(string userId);
        Task SaveChangesAsync();
        Task UpsertAsync(ProfileLocation profile);
    }
}