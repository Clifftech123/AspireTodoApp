using System.Security.Cryptography;
using System.Text;

namespace AspireTodoApp.Server.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }

    public AppUser User { get; set; } = null!;

    public static RefreshToken Create(string userId, string rawToken, DateTime expiresAt) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        TokenHash = Hash(rawToken),
        ExpiresAt = expiresAt,
        CreatedAt = DateTime.UtcNow,
    };

    public static string Hash(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(bytes);
    }

    public void Revoke() => IsRevoked = true;
}
