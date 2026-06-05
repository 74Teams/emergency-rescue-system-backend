using RescueSystem.Application.DTOs.Dispatcher;
using RescueSystem.Application.DTOs.Request;
using RescueSystem.Application.DTOs.RescueTeam;
using RescueSystem.Application.Features.Checklist;
using RescueSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RescueSystem.Application.DTOs.Mission
{
    public class MissionDetailDTO
    {
        public Guid Id { get; set; }

        public RequestDTO? Request { get; set; }
        public RescueTeamDTO? RescueTeam { get; set; }
        public DispatcherDTO? Dispatcher { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public List<ChecklistDetailDTO> Checklists { get; set; } = new();
    }
}
