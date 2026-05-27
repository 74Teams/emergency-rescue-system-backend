using Microsoft.EntityFrameworkCore;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Persistence.Repositories;

internal class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        => await _context.RefreshTokens.AddAsync(token, cancellationToken);

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => _context.RefreshTokens
            .AsNoTracking()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var t in tokens)
            t.RevokedAt = DateTime.UtcNow;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}