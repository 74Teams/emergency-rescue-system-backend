using FluentValidation;

namespace RescueSystem.Application.Features.RescueTeam.Commands.AddMemberToRescueTeam
{
    public class AddMemberToRescueTeamValidator : AbstractValidator<AddMemberToRescueTeamCommand>
    {
        public AddMemberToRescueTeamValidator()
        {
            RuleFor(x => x.TeamId).NotEmpty().WithMessage("Yêu cầu cần có một Đội cứu hộ");
            RuleFor(x => x.MemberId).NotEmpty().WithMessage("Yêu cầu cần có một người");
        }
    }
}