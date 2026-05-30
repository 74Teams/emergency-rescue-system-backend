using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using RescueSystem.Application.DTOs.User;

namespace RescueSystem.Application.DTOs.Commander
{
    public class UserSystemDTO : UserDTO
    {
        public bool IsPendingApproval { get; set; }
        public Guid? RescueTeamId { get; set; }
    }
}
