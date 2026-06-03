using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface IRescueTeamJoinRequestRepository
    {
        Task CreateAsync(RescueTeamJoinRequest request);
        Task<RescueTeamJoinRequest?> GetByIdAsync(Guid id);
        Task<RescueTeamJoinRequest?> GetPendingByRescuerIdAsync(Guid rescuerId);
        Task<List<RescueTeamJoinRequest>> GetPendingByTeamIdAsync(Guid teamId);
        Task<List<RescueTeamJoinRequest>> GetAllPendingAsync();
        Task UpdateAsync(RescueTeamJoinRequest request);
    }
}
