using MediatR;
using System;

namespace RescueSystem.Application.Features.Commander.Commands.ToggleUserStatus
{
    public class ToggleUserStatusCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }
}