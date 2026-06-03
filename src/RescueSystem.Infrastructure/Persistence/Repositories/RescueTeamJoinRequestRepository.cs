using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Entities;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class RescueTeamJoinRequestRepository : IRescueTeamJoinRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public RescueTeamJoinRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(RescueTeamJoinRequest request)
        {
            await _context.RescueTeamJoinRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task<RescueTeamJoinRequest?> GetByIdAsync(Guid id)
        {
            return await _context.RescueTeamJoinRequests
                .Include(r => r.Rescuer)
                .Include(r => r.RescueTeam)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RescueTeamJoinRequest?> GetPendingByRescuerIdAsync(Guid rescuerId)
        {
            return await _context.RescueTeamJoinRequests
                .Include(r => r.RescueTeam)
                .FirstOrDefaultAsync(r => r.RescuerId == rescuerId && r.Status == 0); // 0 = Pending
        }

        public async Task<List<RescueTeamJoinRequest>> GetPendingByTeamIdAsync(Guid teamId)
        {
            return await _context.RescueTeamJoinRequests
                .Include(r => r.Rescuer)
                .Where(r => r.RescueTeamId == teamId && r.Status == 0) // 0 = Pending
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<RescueTeamJoinRequest>> GetAllPendingAsync()
        {
            return await _context.RescueTeamJoinRequests
                .Include(r => r.Rescuer)
                .Include(r => r.RescueTeam)
                .Where(r => r.Status == 0) // 0 = Pending
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(RescueTeamJoinRequest request)
        {
            _context.RescueTeamJoinRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
