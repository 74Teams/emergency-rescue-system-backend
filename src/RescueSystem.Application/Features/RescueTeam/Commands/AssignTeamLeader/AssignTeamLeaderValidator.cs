using System.Security.Cryptography.X509Certificates;
using FluentValidation;

namespace RescueSystem.Application.Features.RescueTeam.Commands.AssignTeamLeader
{
    public class AssignTeamLeaderValidator : AbstractValidator<AssignTeamLeaderCommand>
    {
        public AssignTeamLeaderValidator()
        {
            RuleFor(x => x.TeamId)
                .NotEmpty().WithMessage("Yêu cầu cần có một Đội cứu hộ");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Yêu cầu cần có một người đảm nhận");

        }
    }
}