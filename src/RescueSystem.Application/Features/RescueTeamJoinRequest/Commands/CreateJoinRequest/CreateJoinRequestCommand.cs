using MediatR;
using RescueSystem.Application.DTOs.RescueTeam;
using System;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.CreateJoinRequest
{
    public class CreateJoinRequestCommand : IRequest<RescueTeamJoinRequestDTO>
    {
        public Guid RescuerId { get; set; }
        public Guid RescueTeamId { get; set; }
        public string? Message { get; set; }
    }
}
