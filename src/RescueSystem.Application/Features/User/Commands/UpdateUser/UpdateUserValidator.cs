using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace RescueSystem.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required.")
                .Must(id => id != Guid.Empty).WithMessage("User Id must be a valid GUID.");

            RuleFor(x => x.FullName)
                .MaximumLength(100)
                .When(x => x.FullName != null);

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^0\d{9}$")
                .WithMessage("Phone number is invalid.")
                .When(x => x.PhoneNumber != null);

            RuleFor(x => x.Address)
                .MaximumLength(255)
                .When(x => x.Address != null);

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Now)
                .WithMessage("Date of birth must be in the past.")
                .When(x => x.DateOfBirth != null);

            RuleFor(x => x.Avatar)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("Avatar must be a valid URL.")
                .When(x => x.Avatar != null);
        }
    }
}
