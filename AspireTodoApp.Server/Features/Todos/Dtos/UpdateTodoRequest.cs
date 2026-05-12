using System.ComponentModel.DataAnnotations;

namespace AspireTodoApp.Server.Features.Todos.Dtos;

public record UpdateTodoRequest(
    [Required][MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    bool IsCompleted,
    Guid? CategoryId);
