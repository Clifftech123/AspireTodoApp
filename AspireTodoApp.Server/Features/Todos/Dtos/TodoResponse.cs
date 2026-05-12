namespace AspireTodoApp.Server.Features.Todos.Dtos;

public record TodoResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime CreatedAtUtc,
    DateTime? CompletedAt,
    Guid? CategoryId,
    string? CategoryName);
