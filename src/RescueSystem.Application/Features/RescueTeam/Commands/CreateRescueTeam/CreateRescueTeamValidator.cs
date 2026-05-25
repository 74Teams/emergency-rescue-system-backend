using System.Security.Cryptography.X509Certificates;
using FluentValidation;

namespace RescueSystem.Application.Features.RescueTeam.Command.CreateRescueTeam
{
    public class CreateRescueTeamValidator : AbstractValidator<CreateRescueTeamCommand>
    {
        public CreateRescueTeamValidator()
        {
            RuleFor(x => x.TeamName)
                .NotEmpty().WithMessage("TeamName is required")
                .MaximumLength(256).WithMessage("TeamNameF must not exceed 200 characters");

            RuleFor(x => x.TeamLeaderId)
                .NotEmpty().WithMessage("Team Leader is required");

            RuleFor(x => x.BaseLocationId)
                .NotEmpty().WithMessage("Base Loacation is required");
        }
    }
}