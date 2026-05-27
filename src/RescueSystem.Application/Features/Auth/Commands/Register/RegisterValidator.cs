using FluentValidation;

namespace RescueSystem.Application.Features.Auth.Commands.Register
{
    internal class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên là bắt buộc")
                .MaximumLength(200).WithMessage("Họ tên không được vượt quá 200 ký tự");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email là bắt buộc")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ")
                .MaximumLength(256).WithMessage("Email không được vượt quá 256 ký tự");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Tên đăng nhập là bắt buộc")
                .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự")
                .MaximumLength(50).WithMessage("Tên đăng nhập không được vượt quá 50 ký tự");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại là bắt buộc")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Số điện thoại không hợp lệ");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow).WithMessage("Ngày sinh phải nhỏ hơn hiện tại");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu là bắt buộc")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
                .MaximumLength(100).WithMessage("Mật khẩu không được vượt quá 100 ký tự")
                .Matches(@"(?=.*[A-Za-z])(?=.*\d)").WithMessage("Mật khẩu phải chứa chữ và số");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Địa chỉ không được vượt quá 500 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.Avatar)
                .Must(u => string.IsNullOrWhiteSpace(u) || Uri.IsWellFormedUriString(u, UriKind.Absolute))
                .WithMessage("Avatar phải là URL hợp lệ");

            RuleFor(x => x.Role)
                .Must(r => string.IsNullOrWhiteSpace(r) || 
                           r.Equals("Citizen", StringComparison.OrdinalIgnoreCase) || 
                           r.Equals("Rescuer", StringComparison.OrdinalIgnoreCase) || 
                           r.Equals("Dispatcher", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Vai trò không hợp lệ. Chỉ chấp nhận Citizen, Rescuer, hoặc Dispatcher.");
        }
    }
}
