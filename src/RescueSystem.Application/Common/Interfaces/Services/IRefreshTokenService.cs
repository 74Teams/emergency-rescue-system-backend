namespace RescueSystem.Application.Common.Interfaces.Services;

public interface IRefreshTokenService
{
    /// <returns>Plain refresh token (chỉ trả client 1 lần)</returns>
    Task<string> GenerateAndStoreAsync(Guid userId, CancellationToken cancellationToken = default);
    /// <returns>UserId nếu hợp lệ; null nếu không</returns>
    Task<Guid?> ValidateAsync(string plainToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(string plainToken, string? replacedByHash = null, CancellationToken cancellationToken = default);
    string HashToken(string plainToken);
}