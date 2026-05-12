using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AspireTodoApp.Server.Features.Auth;

internal sealed class JwtTokenService : IJwtTokenService
{
    private const int MinKeyByteLength = 32;

    private readonly JwtSettings _settings;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;

        if (string.IsNullOrWhiteSpace(_settings.Key))
            throw new InvalidOperationException("JWT signing key is not configured.");

        var keyBytes = Encoding.UTF8.GetBytes(_settings.Key);
        if (keyBytes.Length < MinKeyByteLength)
            throw new InvalidOperationException(
                $"JWT signing key must be at least {MinKeyByteLength} bytes (was {keyBytes.Length}).");

        _signingKey = new SymmetricSecurityKey(keyBytes);
    }

    public (string accessToken, DateTime expiresAt) GenerateAccessToken(AppUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("displayName", user.DisplayName ?? string.Empty),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        => ValidateTokenCore(token, validateLifetime: false);

    private ClaimsPrincipal ValidateTokenCore(string token, bool validateLifetime)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new SecurityTokenException("Token is empty.");

        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
            throw new SecurityTokenException("Token is malformed.");

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,
            ValidateAudience = true,
            ValidAudience = _settings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
        };

        try
        {
            var principal = handler.ValidateToken(token, parameters, out var validatedToken);
            EnsureHmacSha256(validatedToken);
            return principal;
        }
        catch (SecurityTokenException)
        {
            throw new SecurityTokenException("Token validation failed.");
        }
    }

    private static void EnsureHmacSha256(SecurityToken validatedToken)
    {
        if (validatedToken is not JwtSecurityToken jwt ||
            !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase) ||
            jwt.SigningKey is null)
            throw new SecurityTokenException("Invalid token algorithm.");
    }
}
