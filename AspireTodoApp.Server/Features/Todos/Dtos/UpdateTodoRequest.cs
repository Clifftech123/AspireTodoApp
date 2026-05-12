namespace AspireTodoApp.Server.Features.Todos.Dtos;

public record UpdateTodoRequest(string Title, string? Description, bool IsCompleted, Guid? CategoryId);
