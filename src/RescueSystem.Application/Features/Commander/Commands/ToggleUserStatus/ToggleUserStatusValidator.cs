using FluentValidation;
using MediatR;
using System;

namespace RescueSystem.Application.Features.Commander.Commands.ToggleUserStatus
{
    public class ToggleUserStatusValidator : AbstractValidator<ToggleUserStatusCommand>
    {
        public ToggleUserStatusValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id is required");
            RuleFor(x => x.IsActive).NotNull().WithMessage("Is active status is required");
        }
    }
}