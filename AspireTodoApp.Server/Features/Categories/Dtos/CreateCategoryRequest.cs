using System.ComponentModel.DataAnnotations;

namespace AspireTodoApp.Server.Features.Categories.Dtos;

public record CreateCategoryRequest([Required][MaxLength(100)] string Name);
