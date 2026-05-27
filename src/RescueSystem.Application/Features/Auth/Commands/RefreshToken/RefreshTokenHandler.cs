using MediatR;
using Microsoft.Extensions.Configuration;
using RescueSystem.Application.Common.Exception;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Application.DTOs.Auth;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;

namespace RescueSystem.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RefreshTokenHandler(
        IRefreshTokenService refreshTokenService,
        IUserRepository userRepository,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _refreshTokenService = refreshTokenService;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = await _refreshTokenService.ValidateAsync(request.RefreshToken, cancellationToken);
        if (userId is null)
            throw new UnauthorizedException("Refresh token không hợp lệ hoặc đã hết hạn");

        var user = await _userRepository.GetUserByIdAsync(userId.Value.ToString());
        if (user is null || !user.IsActive)
            throw new UnauthorizedException("Tài khoản không tồn tại hoặc đã bị khóa");

        // Rotation: revoke cũ, tạo mới
        var newPlainRefresh = await _refreshTokenService.GenerateAndStoreAsync(user.Id, cancellationToken);
        await _refreshTokenService.RevokeAsync(
            request.RefreshToken,
            _refreshTokenService.HashToken(newPlainRefresh),
            cancellationToken);

        var roles = await _userRepository.GetUserRolesAsync(user);
        var accessToken = _tokenService.GenerateToken(user.Id.ToString(), user.Email!, roles);

        var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "30");

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newPlainRefresh,
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