using Microsoft.EntityFrameworkCore;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class ChecklistItemRepository : IChecklistItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ChecklistItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ChecklistItem item)
        {
            await _context.ChecklistItems.AddAsync(item);
        }

        public async Task<ChecklistItem?> GetByIdAsync(Guid id)
        {
            return await _context.ChecklistItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(ChecklistItem item)
        {
            _context.ChecklistItems.Update(item);
        }

        public void Delete(ChecklistItem item)
        {
            _context.ChecklistItems.Remove(item);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<List<ChecklistItem>> GetByChecklistIdAsync(Guid checklistId)
        {
            return await _context.ChecklistItems
                .Where(x => x.ChecklistId == checklistId)
                .ToListAsync();
        }
    }
}
