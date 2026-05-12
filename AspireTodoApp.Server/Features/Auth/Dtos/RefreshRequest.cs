using System.ComponentModel.DataAnnotations;

namespace AspireTodoApp.Server.Features.Auth.Dtos;

public record RefreshRequest([Required] string RefreshToken);
