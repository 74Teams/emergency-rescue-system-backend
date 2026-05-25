using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RescueSystem.Application.Features.Missions.Commands.FinishMission
{
    public class FinishMissionValidator : AbstractValidator<FinishMissionCommand>
    {
        public FinishMissionValidator()
        {
            RuleFor(x => x.MissionId)
                .NotEmpty().When(x => x.MissionId != Guid.Empty)
                .WithMessage("Mission ID is required.")
                .Must(id => id != Guid.Empty).WithMessage("Mission ID must be a valid GUID.");
        }
    }
}
