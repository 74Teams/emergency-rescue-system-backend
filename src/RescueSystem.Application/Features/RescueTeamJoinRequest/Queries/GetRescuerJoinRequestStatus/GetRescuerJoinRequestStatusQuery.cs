using MediatR;
using RescueSystem.Application.DTOs.RescueTeam;
using System;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Queries.GetRescuerJoinRequestStatus
{
    public class GetRescuerJoinRequestStatusQuery : IRequest<RescueTeamJoinRequestDTO?>
    {
        public Guid RescuerId { get; set; }
    }
}
