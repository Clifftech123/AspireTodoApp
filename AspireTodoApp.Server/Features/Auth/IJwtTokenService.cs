using System.Security.Claims;
using AspireTodoApp.Server.Entities;

namespace AspireTodoApp.Server.Features.Auth;

public interface IJwtTokenService
{
    (string accessToken, DateTime expiresAt) GenerateAccessToken(AppUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
