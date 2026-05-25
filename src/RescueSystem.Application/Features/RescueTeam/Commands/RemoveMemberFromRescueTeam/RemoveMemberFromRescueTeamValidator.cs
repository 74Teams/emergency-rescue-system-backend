using FluentValidation;

namespace RescueSystem.Application.Features.RescueTeam.Commands.RemoveMemberFromRescueTeam
{
    public class RemoveMemberFromRescueTeamValidator : AbstractValidator<RemoveMemberFromRescueTeamCommand>
    {
        public RemoveMemberFromRescueTeamValidator()
        {
            RuleFor(x => x.TeamId).NotEmpty().WithMessage("TeamId is required");
            RuleFor(x => x.MemberId).NotEmpty().WithMessage("MemberId is required");
            RuleFor(x => x.MemberId)
               .NotEqual(x => x.TeamId)
               .WithMessage("TeamId and MemberId cannot be the same");
        }
    }
}