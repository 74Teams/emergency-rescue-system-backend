using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Common.Exception;

namespace RescueSystem.Infrastructure.Persistence.Repositories
{
    public class RescueTeamRepository : IRescueTeamRepository
    {
        private readonly ApplicationDbContext _context;
        public RescueTeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(RescueTeam rescueTeam)
        {
            await _context.RescueTeams.AddAsync(rescueTeam);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<RescueTeam>> GetAllAsync()
        {
            return await _context.RescueTeams
                .AsNoTracking()
                .Include(t => t.BaseLocation)
                .Include(t => t.TeamLeader)
                .Include(t => t.Members)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<RescueTeam?> GetByIdAsync(Guid id)
        {
            return await _context.RescueTeams
                .AsNoTracking()
                .Include(t => t.BaseLocation)
                .Include(t => t.TeamLeader)
                .Include(t => t.Members)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> RemoveMemberAsync(Guid teamId, Guid memberId)
        {
            var team = await _context.RescueTeams
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
            {
                throw new NotFoundException("Rescue team not found");
            }

            var member = team.Members.FirstOrDefault(member => member.Id == memberId);
            if (member == null)
            {
                return false;
            }

            team.Members.Remove(member);
            member.RescueTeamId = null;
            team.UpdatedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddMemberAsync(Guid teamId, Guid memberId)
        {
            var team = await _context.RescueTeams
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                throw new NotFoundException("Rescue team not found");
            }

            var member = await _context.Users.FindAsync(memberId);

            if (member == null)
            {
                throw new NotFoundException("User not found");
            }

            if (member.RescueTeamId.HasValue)
            {
                if (member.RescueTeamId == teamId)
                    throw new BadRequestException("User is already a member of the rescue team");
                throw new BadRequestException("User already belongs to another rescue team");
            }

            team.Members.Add(member);
            member.RescueTeamId = teamId;
            team.UpdatedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<ApplicationUser>> GetMembersByTeamIdAsync(Guid teamId)
        {
            var team = await _context.RescueTeams
                .Include(t => t.Members)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                throw new NotFoundException("Rescue team not found");
            }

            return team.Members.ToList();
        }

        public async Task<bool> UpdateTeamStatusAsync(Guid teamId, TeamStatus newStatus)
        {
            var team = await _context.RescueTeams.FindAsync(teamId);

            if (team == null)
            {
                throw new NotFoundException("Rescue team not found");
            }
            team.Status = newStatus;
            team.UpdatedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(RescueTeam rescueTeam)
        {
            var members = await _context.Users.Where(u => u.RescueTeamId == rescueTeam.Id).ToListAsync();
            foreach (var member in members)
            {
                member.RescueTeamId = null;
            }
            _context.RescueTeams.Remove(rescueTeam);

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Mission>> GetMissionsByTeamIdAsync(Guid teamId)
        {
            var team = await _context.RescueTeams
                .AsNoTracking()
                .AnyAsync(t => t.Id == teamId);

            if (!team)
            {
                throw new NotFoundException("Rescue team not found");
            }
            return await _context.Missions
       .AsNoTracking()
       .Include(m => m.Request)
       .Include(m => m.Dispatcher)
       .Include(m => m.RescueTeam)
       .Where(m => m.RescueTeamId == teamId)
       .OrderByDescending(m => m.StartTime)
       .ToListAsync();
        }

        public async Task<bool> UpdateRescueTeamAsync(RescueTeam rescueTeam)
        {
            var existingTeam = await _context.RescueTeams.FirstOrDefaultAsync(t => t.Id == rescueTeam.Id);
            if (existingTeam == null)
            {
                throw new NotFoundException("Rescue team not found");
            }

            existingTeam.TeamName = rescueTeam.TeamName;
            existingTeam.TeamLeaderId = rescueTeam.TeamLeaderId;
            existingTeam.BaseLocationId = rescueTeam.BaseLocationId;
            existingTeam.Status = rescueTeam.Status;
            existingTeam.UpdatedAt = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }
        // public async Task<List<RescueTeam>> GetByStatusAsync(TeamStatus status)
        // {
        //     return await _context.RescueTeams.Where(x => x.Status == status).ToListAsync();
        // }
        // public async Task<bool> CreateAsync(RescueTeam rescueTeam)
        // {
        //     await _context.RescueTeams.AddAsync(rescueTeam);
        //     return await _context.SaveChangesAsync() > 0;
        // }
        // public async Task<bool> UpdateAsync(RescueTeam rescueTeam)
        // {
        //     var existingTeam = await _context.RescueTeams.FirstOrDefaultAsync(x => x.Id == rescueTeam.Id);
        //     if (existingTeam == null)
        //     {
        //         throw new NotFoundException("Rescue team not found");
        //     } 
        //     existingTeam.TeamName = rescueTeam.TeamName;
        //     existingTeam.TeamLeaderId = rescueTeam.TeamLeaderId;
        //     existingTeam.BaseLocationId = rescueTeam.BaseLocationId;
        //     existingTeam.Status = rescueTeam.Status;
        //     existingTeam.UpdatedAt = DateTime.UtcNow;
        //     _context.RescueTeams.Update(rescueTeam);
        //     return await _context.SaveChangesAsync() > 0;
        // }
        // public async Task<bool> DeleteAsync(RescueTeam rescueTeam)
        // {
        //     _context.RescueTeams.Remove(rescueTeam);
        //     return await _context.SaveChangesAsync() > 0;
        // }
    }
}