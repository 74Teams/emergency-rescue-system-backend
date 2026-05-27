using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Common.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(Domain.Entities.RefreshToken token, CancellationToken cancellationToken = default);
        Task<Domain.Entities.RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
