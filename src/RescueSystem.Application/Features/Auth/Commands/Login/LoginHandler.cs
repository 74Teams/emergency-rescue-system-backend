using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Application.DTOs.Auth;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Features.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IConfiguration _configuration;

        public LoginHandler(
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

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var isPasswordValid = await _userRepository.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                throw new BadRequestException("Invalid password or email! Please try again");
            }


            var roles = await _userRepository.GetUserRolesAsync(user);
            var accessToken = _tokenService.GenerateToken(user.Id.ToString(), user.Email, roles);
            var refreshToken = await _refreshTokenService.GenerateAndStoreAsync(user.Id, cancellationToken);
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "30");

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,           // sau này thêm refresh token service
                ExpiresIn = expiryMinutes * 60,
                User = new AuthUserDTO
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Avatar = user.Avatar,
                    Roles = roles,
                },
            };
        }
    }
}
