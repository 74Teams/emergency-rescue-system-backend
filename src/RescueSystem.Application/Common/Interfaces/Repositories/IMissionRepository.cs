using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface IMissionRepository
    {
        Task<Mission?> GetByIdAsync(Guid id);
        Task<IEnumerable<Mission>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<Mission>> GetByStatusAsync(MissionStatus status);
        Task<Guid> AddAsync(Mission mission);
        Task UpdateAsync(Mission mission);
        Task DeleteAsync(Guid id);

        // History
        Task AddHistoryAsync(MissionHistory history);
        Task<IEnumerable<MissionHistory>> GetHistoriesByMissionIdAsync(Guid missionId);

        IQueryable<Mission> Query();
        Task<Mission?> GetByRequestAndTeamAsync(Guid requestId, Guid rescueTeamId);
        Task<Mission?> GetActiveMissionByTeamIdAsync(Guid rescueTeamId);
    }
}
