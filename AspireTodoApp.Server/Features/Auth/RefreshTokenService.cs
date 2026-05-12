using AspireTodoApp.Server.Data;
using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspireTodoApp.Server.Features.Auth;

internal sealed class RefreshTokenService(AppDbContext db, IOptions<JwtSettings> options) : IRefreshTokenService
{
    private readonly JwtSettings _settings = options.Value;

    public async Task<RefreshToken?> GetActiveTokenAsync(string rawToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken)) return null;

        var hash = RefreshToken.Hash(rawToken);
        var now = DateTime.UtcNow;

        return await db.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == hash && !r.IsRevoked && r.ExpiresAt > now, ct);
    }

    public async Task<RefreshToken> CreateAsync(string userId, string rawToken, CancellationToken ct = default)
    {
        var entity = RefreshToken.Create(
            userId,
            rawToken,
            DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays));

        db.RefreshTokens.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<RefreshToken> RotateAsync(
        string oldRawToken,
        string userId,
        string newRawToken,
        CancellationToken ct = default)
    {
        var existing = await GetActiveTokenAsync(oldRawToken, ct)
            ?? throw new InvalidOperationException("Refresh token is invalid or expired.");

        if (existing.UserId != userId)
            throw new InvalidOperationException("Refresh token does not belong to this user.");

        existing.Revoke();

        var replacement = RefreshToken.Create(
            userId,
            newRawToken,
            DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays));

        db.RefreshTokens.Add(replacement);
        await db.SaveChangesAsync(ct);
        return replacement;
    }

    public async Task RevokeAsync(string rawToken, CancellationToken ct = default)
    {
        var existing = await GetActiveTokenAsync(rawToken, ct);
        if (existing is not null)
        {
            existing.Revoke();
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct = default)
    {
        var tokens = await db.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ToListAsync(ct);

        foreach (var t in tokens)
            t.Revoke();

        await db.SaveChangesAsync(ct);
    }
}
