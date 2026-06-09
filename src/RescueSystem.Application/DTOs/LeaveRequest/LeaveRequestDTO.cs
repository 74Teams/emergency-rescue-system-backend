using RescueSystem.Domain.Entities;
using System;

namespace RescueSystem.Application.DTOs.LeaveRequest
{
    public class LeaveRequestDTO
    {
        public Guid Id { get; set; }
        public Guid RescuerId { get; set; }
        public string RescuerName { get; set; } = string.Empty;
        public Guid RescueTeamId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static LeaveRequestDTO FromEntity(Domain.Entities.LeaveRequest entity)
        {
            return new LeaveRequestDTO
            {
                Id = entity.Id,
                RescuerId = entity.RescuerId,
                RescuerName = entity.Rescuer?.FullName ?? string.Empty,
                RescueTeamId = entity.RescueTeamId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Reason = entity.Reason,
                Status = entity.Status,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
