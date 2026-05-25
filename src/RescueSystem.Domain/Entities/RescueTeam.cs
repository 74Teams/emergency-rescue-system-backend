using RescueSystem.Domain.Entities.Base;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Domain.Entities
{
    public class RescueTeam : BaseEntities
    {
        public string TeamName { get; set; } = string.Empty;

        public Guid TeamLeaderId { get; set; }

        public Guid BaseLocationId { get; set; }

        public string? Description {get; set;}

        public TeamStatus Status { get; set; } = TeamStatus.AVAILABLE;

        // Navigation Properties
        public ApplicationUser? TeamLeader { get; set; }
        public Location? BaseLocation { get; set; }
        public ICollection<ApplicationUser> Members { get; set; } = new List<ApplicationUser>();
        public ICollection<Mission> Missions { get; set; } = new List<Mission>();
    }
}
