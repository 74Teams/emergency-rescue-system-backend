using MediatR;
using RescueSystem.Application.DTOs.RescueTeam;
using System;
using System.Collections.Generic;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Queries.GetTeamJoinRequests
{
    public class GetTeamJoinRequestsQuery : IRequest<List<RescueTeamJoinRequestDTO>>
    {
        public Guid? TeamId { get; set; }
        public Guid UserId { get; set; }
    }
}
