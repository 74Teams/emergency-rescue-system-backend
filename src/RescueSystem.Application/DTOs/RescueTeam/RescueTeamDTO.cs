using System;
using System.Collections.Generic;
using System.Text;
using RescueSystem.Application.DTOs.Location;
using RescueSystem.Application.DTOs.User;

namespace RescueSystem.Application.DTOs.RescueTeam
{
    public class RescueTeamDTO
    {
        public Guid Id { get; set; }
        public string? TeamName { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public Guid? LeaderId { get; set; }
        public LocationDTO? BaseLocation { get; set; }
        public UserDTO? Leader { get; set; }
        public int MemberCount { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
