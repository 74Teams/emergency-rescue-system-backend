using MediatR;
using System;

namespace RescueSystem.Application.Features.Missions.Commands.DeleteMission
{
    public class DeleteMissionCommand : IRequest<bool>
    {
        public Guid MissionId { get; set; }
    }
}
