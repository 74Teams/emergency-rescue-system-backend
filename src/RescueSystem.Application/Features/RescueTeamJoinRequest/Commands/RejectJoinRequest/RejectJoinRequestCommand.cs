using MediatR;
using System;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.RejectJoinRequest
{
    public class RejectJoinRequestCommand : IRequest<bool>
    {
        public Guid RequestId { get; set; }
        public Guid RejectedById { get; set; }
    }
}
