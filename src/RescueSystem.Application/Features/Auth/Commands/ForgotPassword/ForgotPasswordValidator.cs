using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Vui lòng nhập Email")
                    .EmailAddress().WithMessage("Email không hợp lệ");
        }
    }
}
