namespace RescueSystem.Application.DTOs.RescueTeam
{
    public class RescueTeamMemberDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public List<string> Roles { get; set; } = new();
    }
}