using System;
using System.Collections.Generic;
using System.Text;
using RescueSystem.Application.DTOs.Base;
using RescueSystem.Application.DTOs.Location;
using RescueSystem.Application.DTOs.User;
using LocationEntity = RescueSystem.Domain.Entities.Location;
using RescueSystem.Domain.Enums;
using RescueSystem.Application.DTOs.Mission;


namespace RescueSystem.Application.DTOs.Request
{
    public class RequestDTO : BaseDTO
    {

        public UserDTO? RequestedBy { get; set; }
        public EmergencyType EmergencyType { get; set; }
        public Priority Priority { get; set; }
        public RequestStatus Status { get; set; }

        // FIXED: Use LocationDTO instead of entity
        public LocationDTO? Location { get; set; }

        public string Description { get; set; } = string.Empty;
        public List<RequestMediaDTO> Medias { get; set; } = new();
        public List<MissionBriefDto> Missions { get; set; } = new();
        // public UserDTO? RequestedBy { get; set; }

        // public EmergencyType EmergencyType { get; set; }

        // public Priority Priority { get; set; }

        // public RequestStatus Status { get; set; } = RequestStatus.PENDING;

        // public LocationEntity? Location { get; set; }

        // public string Description { get; set; } = string.Empty;
        // public List<RequestMediaDTO> Medias { get; set; } = new List<RequestMediaDTO>();
        // public List<MissionBriefDto> Missions { get; set; } = new();
        public class MissionBriefDto
        {
            public Guid Id { get; set; }
            public MissionStatus Status { get; set; }
            public Guid RescueTeamId { get; set; }
            public string? TeamName { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
        }
    }

    public class NonRelationRequestDTO : BaseDTO
    {
        public EmergencyType EmergencyType { get; set; }

        public Priority Priority { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.PENDING;
        public LocationEntity? Location { get; set; }

    }


}
