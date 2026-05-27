using Microsoft.EntityFrameworkCore;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class ChecklistRepository : IChecklistRepository
    {
        private readonly ApplicationDbContext _context;
        public ChecklistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Checklist checklist)
        {
            await _context.Checklists.AddAsync(checklist);
        }

        public async Task<Checklist?> GetByIdAsync(Guid id)
        {
            return await _context.Checklists
                .AsNoTracking()
                .Include(x => x.ChecklistItems)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Checklist>> GetAllAsync()
        {
            return await _context.Checklists.AsNoTracking().ToListAsync();
        }

        public void Update(Checklist checklist)
        {
            _context.Checklists.Update(checklist);
        }

        public void Delete(Checklist checklist)
        {
            _context.Checklists.Remove(checklist);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
