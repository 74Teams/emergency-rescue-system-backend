using MediatR;
using RescueSystem.Application.Common.Response;
using RescueSystem.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.User.Commands
{
    public class CreateUserCommand : IRequest<Unit>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
