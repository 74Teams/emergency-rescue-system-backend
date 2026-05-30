using System.Security.Cryptography.X509Certificates;
using FluentValidation;

namespace RescueSystem.Application.Features.RescueTeam.Commands.CreateRescueTeam
{
    public class CreateRescueTeamValidator : AbstractValidator<CreateRescueTeamCommand>
    {
        public CreateRescueTeamValidator()
        {
            RuleFor(x => x.TeamName)
                .NotEmpty().WithMessage("Cần có tên Đội cứu hộ")
                .MaximumLength(256).WithMessage("Tên đội cứu hộ không vượt quá 256 kí tự");

            RuleFor(x => x.TeamLeaderId)
                .NotEmpty().WithMessage("Yêu cầu có một Team leader");

            RuleFor(x => x.BaseLocationId)
                .NotEmpty().WithMessage("Cần có địa chỉ trụ sở");
        }
    }
}