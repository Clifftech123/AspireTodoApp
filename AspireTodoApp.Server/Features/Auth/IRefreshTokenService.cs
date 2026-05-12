using AspireTodoApp.Server.Entities;

namespace AspireTodoApp.Server.Features.Auth;

public interface IRefreshTokenService
{
    Task<RefreshToken?> GetActiveTokenAsync(string rawToken, CancellationToken ct = default);
    Task<RefreshToken> CreateAsync(string userId, string rawToken, CancellationToken ct = default);
    Task<RefreshToken> RotateAsync(string oldRawToken, string userId, string newRawToken, CancellationToken ct = default);
    Task RevokeAsync(string rawToken, CancellationToken ct = default);
    Task RevokeAllForUserAsync(string userId, CancellationToken ct = default);
}
