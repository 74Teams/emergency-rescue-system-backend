using MediatR;
using RescueSystem.Application.DTOs.Auth;
using System;

namespace RescueSystem.Application.Features.Auth.Commands.SelectRole
{
    public class SelectRoleCommand : IRequest<AuthResponse>
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
