using RescueSystem.Application.DTOs.Common;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface IRequestRespository
    {
        Task<List<RescueRequest>> GetAllAsync();
        Task<RescueRequest?> GetByIdAsync(Guid id);
        Task<List<RescueRequest>> GetByUserIdAsync(Guid userId);
        Task<List<RescueRequest>> GetByStatusAsync(RequestStatus status);
        Task<PagedResult<RescueRequest>> GetPagedAsync(int page, int pageSize, RequestStatus? status = null, Priority? priority = null, EmergencyType? emergencyType = null, string? sortBy = null);

        Task CreateAsync(RescueRequest request);
        Task UpdateAsync(RescueRequest request);
        Task UpdateStatusAsync(Guid requestId, RequestStatus status);
        Task DeleteAsync(RescueRequest request);
    }
}
