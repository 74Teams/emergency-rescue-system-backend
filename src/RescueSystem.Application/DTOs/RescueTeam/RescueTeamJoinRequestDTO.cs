using System;

namespace RescueSystem.Application.DTOs.RescueTeam
{
    public class RescueTeamJoinRequestDTO
    {
        public Guid Id { get; set; }
        public Guid RescuerId { get; set; }
        public string RescuerName { get; set; } = string.Empty;
        public string RescuerEmail { get; set; } = string.Empty;
        public string RescuerPhone { get; set; } = string.Empty;
        public string RescuerAvatar { get; set; } = string.Empty;
        public Guid RescueTeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int Status { get; set; } // 0 = Pending, 1 = Approved, 2 = Rejected
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
