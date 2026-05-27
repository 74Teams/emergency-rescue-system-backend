using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface IChecklistItemRepository
    {
        Task AddAsync(ChecklistItem item);
        Task<ChecklistItem?> GetByIdAsync(Guid id);
        void Update(ChecklistItem item);
        void Delete(ChecklistItem item);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<List<ChecklistItem>> GetByChecklistIdAsync(Guid checklistId);
    }
}
