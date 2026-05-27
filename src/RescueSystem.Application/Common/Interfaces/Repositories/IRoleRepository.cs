using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<List<ApplicationRole>> GetAllAsync();
        Task<ApplicationRole?> GetByIdAsync(Guid id);
        Task<ApplicationRole?> GetByNameAsync(string name);

        Task<bool> CreateAsync(ApplicationRole role);
        Task<bool> UpdateAsync(ApplicationRole role);
        Task<bool> DeleteAsync(ApplicationRole role);
    }
}
