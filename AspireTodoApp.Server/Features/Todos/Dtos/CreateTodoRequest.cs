namespace AspireTodoApp.Server.Features.Todos.Dtos;

public record CreateTodoRequest(string Title, string? Description, Guid? CategoryId);
