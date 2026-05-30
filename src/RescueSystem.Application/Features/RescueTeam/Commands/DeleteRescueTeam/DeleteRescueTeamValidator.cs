using FluentValidation;

namespace RescueSystem.Application.Features.RescueTeam.Commands.DeleteRescueTeam
{
    public class DeleteRescueTeamValidator : AbstractValidator<DeleteRescueTeamCommand>
    {
        public DeleteRescueTeamValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Yêu cầu có một Rescue Team");
        }
    }
}