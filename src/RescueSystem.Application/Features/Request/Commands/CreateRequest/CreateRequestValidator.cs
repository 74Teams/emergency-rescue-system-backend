using FluentValidation;
using RescueSystem.Application.DTOs.Request;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.Features.Request.Commands.CreateRequest
{
    public class CreateRequestValidator : AbstractValidator<CreateRequestCommand>
    {
        public CreateRequestValidator()
        {
            RuleFor(x => x.UserId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("UserId không hợp lệ")
                .When(x => x.UserId.HasValue);

            RuleFor(x => x.EmergencyType)
                .IsInEnum().WithMessage("Loại tình huống khẩn cấp không hợp lệ");

            RuleFor(x => x.Status)
                .Equal(RequestStatus.PENDING)
                .WithMessage("Trạng thái ban đầu của yêu cầu phải là PENDING");

            RuleFor(x => x.LocationId)
                .NotEmpty().WithMessage("LocationId là bắt buộc");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả không được bỏ trống")
                .MaximumLength(2000).WithMessage("Mô tả không được vượt quá 2000 ký tự");

            RuleFor(x => x.Files)
                .Must(files => files == null || files.Count <= 10)
                .WithMessage("Số lượng file tối đa là 10");
            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Mức độ ưu tiên không hợp lệ");
        }
    }
}
