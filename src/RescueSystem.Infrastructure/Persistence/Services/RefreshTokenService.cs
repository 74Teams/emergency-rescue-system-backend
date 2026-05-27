using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Persistence.Services;

internal class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repository;
    private readonly IConfiguration _config;

    public RefreshTokenService(IRefreshTokenRepository repository, IConfiguration config)
    {
        _repository = repository;
        _config = config;
    }

    public async Task<string> GenerateAndStoreAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var plainToken = GenerateSecureToken();
        var hash = HashToken(plainToken);

        var days = int.Parse(_config["JwtSettings:RefreshTokenExpiryDays"] ?? "7");

        await _repository.AddAsync(new RefreshToken
        {
            UserId = userId,
            TokenHash = hash,
            ExpiresAt = DateTime.UtcNow.AddDays(days),
        }, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);
        return plainToken;
    }

    public async Task<Guid?> ValidateAsync(string plainToken, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByHashAsync(HashToken(plainToken), cancellationToken);
        if (entity is null || !entity.IsActive)
            return null;

        return entity.UserId;
    }

    public async Task RevokeAsync(string plainToken, string? replacedByHash = null, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByHashAsync(HashToken(plainToken), cancellationToken);
        if (entity is null) return;

        entity.RevokedAt = DateTime.UtcNow;
        entity.ReplacedByTokenHash = replacedByHash;
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string plainToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainToken));
        return Convert.ToHexString(bytes);
    }
}