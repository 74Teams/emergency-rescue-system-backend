using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Checklist.Commands.CreateChecklist
{
    public class CreateChecklistValidator : AbstractValidator<CreateChecklistCommand>
    {
        public CreateChecklistValidator()
        {
            RuleFor(x => x.Title)
               .NotEmpty()
               .MaximumLength(256);
        }
    }
}
