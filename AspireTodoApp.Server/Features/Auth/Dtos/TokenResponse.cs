namespace AspireTodoApp.Server.Features.Auth.Dtos;

public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
