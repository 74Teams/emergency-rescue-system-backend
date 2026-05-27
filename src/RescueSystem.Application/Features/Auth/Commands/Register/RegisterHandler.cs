using MediatR;
using RescueSystem.Application.Common.Enums;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Application.DTOs.Auth;
using RescueSystem.Application.Interfaces.Respositories;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Features.Auth.Commands.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public RegisterHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("Email đã được sử dụng");
            }

           

            var role = string.IsNullOrWhiteSpace(request.Role) ? RoleEnum.Citizen.ToString() : request.Role;
            var roles = new List<string> { role };
            var isCitizen = string.Equals(role, RoleEnum.Citizen.ToString(), StringComparison.OrdinalIgnoreCase);
            var isPendingApproval = string.Equals(role, RoleEnum.Dispatcher.ToString(), StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, RoleEnum.Rescuer.ToString(), StringComparison.OrdinalIgnoreCase);
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                Avatar = request.Avatar,
                IsActive = isCitizen,
                IsPendingApproval = isPendingApproval,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var result = await _userRepository.CreateUserAsync(user, request.Password, roles);

            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return Unit.Value;
        }
    }
}
