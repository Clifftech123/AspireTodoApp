namespace AspireTodoApp.Server.Features.Auth.Dtos;

public record RegisterRequest(string Email, string Password, string DisplayName);
