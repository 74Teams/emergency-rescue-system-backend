using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface IContactRepository
    {
        Task<Contact?> GetByIdAsync(Guid id);
        Task<List<Contact>> GetByUserIdAsync(Guid userId);
        Task CreateAsync(Contact contact);
        Task UpdateAsync(Contact contact);
        Task DeleteAsync(Contact contact);
    }
}
