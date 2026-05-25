using System.Security.Cryptography.X509Certificates;
using FluentValidation;

namespace RescueSystem.Application.Features.RescueTeam.Commands.AssignTeamLeader
{
    public class AssignTeamLeaderValidator:AbstractValidator<AssignTeamLeaderCommand> 
    {
        public AssignTeamLeaderValidator() {
            RuleFor(x=>x.TeamId)
                .NotEmpty().WithMessage("Team is required");

            RuleFor(x=>x.UserId)
                .NotEmpty().WithMessage("User is required");

        }
    }
}