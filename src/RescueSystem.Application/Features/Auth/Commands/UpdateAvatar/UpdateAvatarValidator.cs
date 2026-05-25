using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Auth.Commands.UpdateAvatar
{
    public class UpdateAvatarValidator : AbstractValidator<UpdateAvatarCommand>
    {
        public UpdateAvatarValidator()
        {

        }
    }
}
