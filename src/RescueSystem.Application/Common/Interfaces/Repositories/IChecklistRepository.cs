using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface IChecklistRepository
    {
        Task AddAsync(Checklist checklist);
        Task<Checklist?> GetByIdAsync(Guid id);
        Task<List<Checklist>> GetAllAsync();
        void Update(Checklist checklist);
        void Delete(Checklist checklist);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
