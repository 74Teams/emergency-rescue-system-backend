using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Missions.Commands.AbortMission
{
    public class AbortMissionValidator : AbstractValidator<AbortMissionCommand>
    {
        public AbortMissionValidator()
        {
            RuleFor(x => x.MissionId)
                .NotEmpty().When(x => x.MissionId != Guid.Empty)
                .WithMessage("Mission ID is required.")
                .Must(id => id != Guid.Empty).WithMessage("Mission ID must be a valid GUID.");
        }
    }
}
