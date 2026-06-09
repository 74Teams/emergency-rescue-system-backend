using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RescueSystem.Application.Common.Interfaces.Repositories
{
    public interface ILeaveRequestRepository
    {
        Task<LeaveRequest> GetByIdAsync(Guid id);
        Task<IEnumerable<LeaveRequest>> GetByRescuerIdAsync(Guid rescuerId);
        Task<IEnumerable<LeaveRequest>> GetByTeamIdAsync(Guid teamId);
        Task CreateAsync(LeaveRequest request);
        Task UpdateAsync(LeaveRequest request);
    }
}
