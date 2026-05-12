using System.ComponentModel.DataAnnotations;

namespace AspireTodoApp.Server.Features.Auth.Dtos;

public record LoginRequest(
    [Required][EmailAddress][MaxLength(256)] string Email,
    [Required] string Password);
