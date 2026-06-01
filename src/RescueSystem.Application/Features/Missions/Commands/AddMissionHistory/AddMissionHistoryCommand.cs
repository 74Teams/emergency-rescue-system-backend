using MediatR;
using RescueSystem.Domain.Enums;
using System;

namespace RescueSystem.Application.Features.Missions.Commands.AddMissionHistory
{
    public class AddMissionHistoryCommand : IRequest<bool>
    {
        public Guid MissionId { get; set; }
        public Guid ChangedById { get; set; }
        public string Note { get; set; }
    }
}
