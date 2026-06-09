using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest> GetByIdAsync(Guid id)
        {
            return await _context.LeaveRequests
                .Include(x => x.Rescuer)
                .Include(x => x.RescueTeam)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<LeaveRequest>> GetByRescuerIdAsync(Guid rescuerId)
        {
            return await _context.LeaveRequests
                .Include(x => x.Rescuer)
                .Include(x => x.RescueTeam)
                .Where(x => x.RescuerId == rescuerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetByTeamIdAsync(Guid teamId)
        {
            return await _context.LeaveRequests
                .Include(x => x.Rescuer)
                .Include(x => x.RescueTeam)
                .Where(x => x.RescueTeamId == teamId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateAsync(LeaveRequest request)
        {
            await _context.LeaveRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
