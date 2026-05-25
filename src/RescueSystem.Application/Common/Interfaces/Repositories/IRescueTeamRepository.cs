using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.Common.Interfaces.Repositories
{
    public interface IRescueTeamRepository
    {
        Task<List<RescueTeam>> GetAllAsync();
        Task<List<Mission>> GetMissionsByTeamIdAsync(Guid id);
        Task<RescueTeam?> GetByIdAsync(Guid id);
        // Task<List<RescueTeam>> GetByStatusAsync(TeamStatus status);
        Task<bool> CreateAsync(RescueTeam rescueTeam);
        Task<bool> RemoveMemberAsync(Guid teamId, Guid memberId);
        Task<bool> AddMemberAsync(Guid teamId, Guid memberId);
        Task<List<ApplicationUser>> GetMembersByTeamIdAsync(Guid teamId);

        Task<bool> UpdateRescueTeamAsync(RescueTeam rescueTeam);
        Task<bool> UpdateTeamStatusAsync(Guid teamId, TeamStatus newStatus);
        // Task<bool> UpdateAsync(RescueTeam rescueTeam);
        Task<bool> DeleteAsync(RescueTeam rescueTeam);
    }
}