using FluentValidation;
using MediatR;
using RescueSystem.Application.Features.Commander.Commands.ToggleUserStatus;
using System;

namespace RescueSystem.Application.Features.Commander.Commands.ApproveUser
{
    public class ApproveUserValidator : AbstractValidator<ApproveUserCommand>
    {
        public ApproveUserValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id is required");
        }
    }
}