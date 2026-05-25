using FluentValidation;
using MediatR;
using System;

namespace RescueSystem.Application.Features.Commander.Commands.RejectUser
{
    public class RejectUserValidator : AbstractValidator<RejectUserCommand>
    {
        public RejectUserValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id is required");
        }
    }
}