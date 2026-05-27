using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(Location location);
        Task<bool> UpdateAsync(Location location);
        Task<bool> DeleteAsync(Location location);
    }
}
