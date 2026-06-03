using MediatR;
using System;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.ApproveJoinRequest
{
    public class ApproveJoinRequestCommand : IRequest<bool>
    {
        public Guid RequestId { get; set; }
        public Guid ApprovedById { get; set; }
    }
}
