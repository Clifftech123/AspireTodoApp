using System.ComponentModel.DataAnnotations;

namespace AspireTodoApp.Server.Features.Todos.Dtos;

public record CreateTodoRequest(
    [Required][MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    Guid? CategoryId);
