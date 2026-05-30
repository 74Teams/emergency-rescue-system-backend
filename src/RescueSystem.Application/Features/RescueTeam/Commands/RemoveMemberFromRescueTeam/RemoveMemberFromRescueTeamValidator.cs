using FluentValidation;

namespace RescueSystem.Application.Features.RescueTeam.Commands.RemoveMemberFromRescueTeam
{
    public class RemoveMemberFromRescueTeamValidator : AbstractValidator<RemoveMemberFromRescueTeamCommand>
    {
        public RemoveMemberFromRescueTeamValidator()
        {
            RuleFor(x => x.TeamId).NotEmpty().WithMessage("Cần chọn đội cứu hộ để thực hiện chức năng");
            RuleFor(x => x.MemberId).NotEmpty().WithMessage("Cần chọn thành viên để thực hiện chức năng");
        }
    }
}