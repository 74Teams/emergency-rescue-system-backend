using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class MissionRepository : IMissionRepository
    {
        private readonly ApplicationDbContext _context;

        public MissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddAsync(Mission mission)
        {
            var req = await _context.Requests.FindAsync(mission.RequestId);
            if (req == null) throw new NotFoundException("Không tìm thấy yêu cầu");
            var dispatcher = await _context.Users.FindAsync(mission.DispatcherId);
            if (dispatcher == null) throw new NotFoundException("Không tìm thấy điều phối viên");
            var rescueTeam = await _context.RescueTeams.FindAsync(mission.RescueTeamId);
            if (rescueTeam == null) throw new NotFoundException("Không tìm thấy đội cứu hộ");
            await _context.Missions.AddAsync(mission);
            await _context.SaveChangesAsync();

            return mission.Id;
        }

        public async Task UpdateAsync(Mission mission)
        {
            _context.Entry(mission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var mission = await _context.Missions.FindAsync(id);
            if (mission == null) return;

            _context.Missions.Remove(mission);
            await _context.SaveChangesAsync();
        }

        public async Task AddHistoryAsync(MissionHistory history)
        {
            await _context.MissionHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MissionHistory>> GetHistoriesByMissionIdAsync(Guid missionId)
        {
            return await _context.MissionHistories
                .AsNoTracking()
                .Include(mh => mh.ChangedBy)
                .Where(mh => mh.MissionId == missionId)
                .OrderByDescending(mh => mh.CreatedAt)
                .ToListAsync();
        }

        public async Task<Mission?> GetByIdAsync(Guid id)
        {
            return await _context.Missions
            .AsNoTracking()
            .Include(x => x.Request)
                .ThenInclude(r => r!.Location)
            .Include(x => x.Request)
                .ThenInclude(r => r!.RequestedBy)
            .Include(x => x.Request)
                .ThenInclude(r => r!.Medias)
            .Include(x => x.Dispatcher)
            .Include(x => x.RescueTeam)
            .Include(x => x.Checklists)
                .ThenInclude(c => c.ChecklistItems)
            .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Mission>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Missions
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mission>> GetByStatusAsync(MissionStatus status)
        {
            return await _context.Missions
                .AsNoTracking()
                .Where(x => x.Status == status)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
        }

        public IQueryable<Mission> Query()
        {
            return _context.Missions
                .Include(x => x.Request)
                    .ThenInclude(r => r!.Location)
                .Include(x => x.RescueTeam)
                .Include(x => x.Dispatcher)
                .AsQueryable();
        }

        public async Task<Mission?> GetByRequestAndTeamAsync(Guid requestId, Guid rescueTeamId)
        {
            return await _context.Missions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RequestId == requestId && x.RescueTeamId == rescueTeamId);
        }

        public async Task<Mission?> GetActiveMissionByTeamIdAsync(Guid rescueTeamId)
        {
            return await _context.Missions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RescueTeamId == rescueTeamId
                    && x.Status != MissionStatus.COMPLETED
                    && x.Status != MissionStatus.ABORTED);
        }
    }
}