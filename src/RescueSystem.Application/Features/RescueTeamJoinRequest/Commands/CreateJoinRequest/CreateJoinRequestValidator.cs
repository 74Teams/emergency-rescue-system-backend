using FluentValidation;

namespace RescueSystem.Application.Features.RescueTeamJoinRequest.Commands.CreateJoinRequest
{
    public class CreateJoinRequestValidator : AbstractValidator<CreateJoinRequestCommand>
    {
        public CreateJoinRequestValidator()
        {
            RuleFor(x => x.RescuerId)
                .NotEmpty().WithMessage("Cần cung cấp thông tin Cứu hộ viên.");

            RuleFor(x => x.RescueTeamId)
                .NotEmpty().WithMessage("Cần chọn Đội cứu hộ để gia nhập.");

            RuleFor(x => x.Message)
                .MaximumLength(500).WithMessage("Lời nhắn không được vượt quá 500 ký tự.");
        }
    }
}
