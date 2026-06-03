using MediatR;
using Microsoft.Extensions.Configuration;
using RescueSystem.Application.Common.Enums;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Application.DTOs.Auth;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RescueSystem.Application.Features.Auth.Commands.SelectRole
{
    public class SelectRoleHandler : IRequestHandler<SelectRoleCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IConfiguration _configuration;

        public SelectRoleHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _configuration = configuration;
        }

        public async Task<AuthResponse> Handle(SelectRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            // Verify the role is valid
            if (!Enum.TryParse<RoleEnum>(request.Role, true, out var roleEnum))
            {
                throw new BadRequestException("Vai trò không hợp lệ");
            }

            var roleName = roleEnum.ToString();

            // Commander is not allowed to be self-selected during signup
            if (roleEnum == RoleEnum.Commander)
            {
                throw new BadRequestException("Không thể tự chọn vai trò Chỉ huy (Commander)");
            }

            var isCitizen = roleEnum == RoleEnum.Citizen;
            var isPendingApproval = roleEnum == RoleEnum.Dispatcher || roleEnum == RoleEnum.Rescuer;

            user.IsActive = isCitizen; // Dispatcher & Rescuer are inactive until approved by Commander
            user.IsPendingApproval = isPendingApproval;
            user.UpdatedAt = DateTime.UtcNow;

            // Update user roles and save user status
            await _userRepository.UpdateUserRolesAsync(user.Id, new List<string> { roleName });
            await _userRepository.UpdateUserAsync(user);

            // Generate tokens for the selected role session
            var roles = new List<string> { roleName };
            var accessToken = _tokenService.GenerateToken(user.Id.ToString(), user.Email, roles);
            var refreshToken = await _refreshTokenService.GenerateAndStoreAsync(user.Id, cancellationToken);
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "30");

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiryMinutes * 60,
                User = new AuthUserDTO
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Avatar = user.Avatar,
                    Roles = roles,
                    IsActive = user.IsActive,
                    IsPendingApproval = user.IsPendingApproval
                }
            };
        }
    }
}
