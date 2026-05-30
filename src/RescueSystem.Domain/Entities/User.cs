using Microsoft.AspNetCore.Identity;

namespace RescueSystem.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public new string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public string AvatarPublicId { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsPendingApproval { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<RescueRequest> Requests { get; set; } = new List<RescueRequest>();
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public Guid? RescueTeamId { get; set; }

        public RescueTeam? MemberOfTeam { get; set; }

        public ICollection<RescueTeam> LeadingTeams { get; set; }
            = new List<RescueTeam>();
        public ICollection<Mission> DispatchedMissions { get; set; } = new List<Mission>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
