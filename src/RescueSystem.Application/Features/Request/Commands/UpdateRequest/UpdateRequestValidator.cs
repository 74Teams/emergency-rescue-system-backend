using FluentValidation;
using Microsoft.AspNetCore.Http;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Application.Features.Request.Commands.UpdateRequest
{
    public class UpdateRequestValidator : AbstractValidator<UpdateRequestCommand>
    {
        public UpdateRequestValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty().WithMessage("Request gửi đi không hợp lệ");

            RuleFor(x => x.UserId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("Người gửi không hợp lệ")
                .When(x => x.UserId.HasValue);

            RuleFor(x => x.EmergencyType)
                .IsInEnum().WithMessage("Loại tình huống khẩn cấp không hợp lệ");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Độ ưu tiên không hợp lệ");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Trạng thái không hợp lệ");

            RuleFor(x => x.LocationId)
                .NotEmpty().WithMessage("LocationId là bắt buộc");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả không được bỏ trống")
                .MaximumLength(2000).WithMessage("Mô tả không được vượt quá 2000 ký tự");

            RuleFor(x => x.Files)
                .Must(files => files == null || files.Count <= 10)
                .WithMessage("Số lượng file tối đa là 10");
        }
    }
}
