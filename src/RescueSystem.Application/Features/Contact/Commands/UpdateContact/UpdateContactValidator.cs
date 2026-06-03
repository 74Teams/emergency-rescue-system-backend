using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Contact.Commands.UpdateContact
{
    public class UpdateContactValidator : AbstractValidator<UpdateContactCommand>
    {
        public UpdateContactValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id không được để trống")
                .NotEqual(Guid.Empty).WithMessage("Id không hợp lệ");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên không được để trống")
                .MaximumLength(100).WithMessage("Tên tối đa 100 ký tự");

            RuleFor(x => x.Relationship)
                .NotEmpty().WithMessage("Mối quan hệ không được để trống")
                .MaximumLength(50);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^0\d{9}$")
                .WithMessage("Số điện thoại không hợp lệ");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email không hợp lệ")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}
