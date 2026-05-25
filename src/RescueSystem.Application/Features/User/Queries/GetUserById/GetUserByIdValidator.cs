using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.User.Queries.GetUserById
{
    public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdValidator()
        {
            RuleFor(x => x.Id)
           .NotEmpty().WithMessage("User ID is required.")
           .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
        }
    }
}
