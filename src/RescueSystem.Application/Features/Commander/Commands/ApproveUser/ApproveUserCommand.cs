using MediatR;
using System;

namespace RescueSystem.Application.Features.Commander.Commands.ApproveUser
{
    public class ApproveUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }
}