using RescueSystem.Domain.Entities.Base;
using System;

namespace RescueSystem.Domain.Entities
{
    public class LeaveRequest : BaseEntities
    {
        public Guid RescuerId { get; set; }
        public Guid RescueTeamId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int Status { get; set; } = 0; // 0 = Pending, 1 = Approved, 2 = Rejected
        public string? Note { get; set; }

        // Navigation Properties
        public ApplicationUser? Rescuer { get; set; }
        public RescueTeam? RescueTeam { get; set; }
    }
}
