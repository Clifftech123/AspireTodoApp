using System.ComponentModel.DataAnnotations;

namespace AspireTodoApp.Server.Features.Auth.Dtos;

public record RegisterRequest(
    [Required][EmailAddress][MaxLength(256)] string Email,
    [Required][MinLength(6)][MaxLength(100)] string Password,
    [Required][MaxLength(100)] string DisplayName);
