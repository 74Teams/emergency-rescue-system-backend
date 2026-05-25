using System;
using System.Collections.Generic;
using System.Text;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.DTOs.RescueTeam
{
    public class CreateRescueTeamDTO
    {
        public string TeamName { get; set; } = string.Empty;
        public Guid TeamLeaderId { get; set; }
        public Guid BaseLocationId { get; set; }

    }
}

