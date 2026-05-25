using MediatR;
using Microsoft.AspNetCore.Http;
using RescueSystem.Application.DTOs.Avatar;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Auth.Commands.UpdateAvatar
{
    public class UpdateAvatarCommand : IRequest<AvatarDTO>
    {
        public IFormFile File { get; set; }
        public Guid UserId { get; set; }
    }
}
