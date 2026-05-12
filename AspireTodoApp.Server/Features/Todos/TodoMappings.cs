using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Features.Todos.Dtos;

namespace AspireTodoApp.Server.Features.Todos;

public static class TodoMappings
{
    public static TodoResponse ToResponse(this Todo todo) => new(
        todo.Id,
        todo.Title,
        todo.Description,
        todo.IsCompleted,
        todo.CreatedAtUtc,
        todo.CompletedAt,
        todo.CategoryId,
        todo.Category?.Name);

    public static CreateTodoRequest ToEntity(this Todo todo) => new(
        todo.Title,
        todo.Description,
        todo.CategoryId);
}
