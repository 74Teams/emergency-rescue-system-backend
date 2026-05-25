using MediatR;
using System;

namespace RescueSystem.Application.Features.Commander.Commands.RejectUser
{
    public class RejectUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }
}