namespace AspireTodoApp.Server.Features.Categories.Dtos;

public record CategoryResponse(Guid Id, string Name, int TodoCount);
