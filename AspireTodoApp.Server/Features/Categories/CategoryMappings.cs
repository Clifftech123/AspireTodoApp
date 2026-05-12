using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Features.Categories.Dtos;

namespace AspireTodoApp.Server.Features.Categories;

public static class CategoryMappings
{
    public static CategoryResponse ToResponse(this Category category, int todoCount = 0) => new(
        category.Id,
        category.Name,
        todoCount);
    


    public static CreateCategoryRequest ToEntity(this Category category) => new(
        category.Name);
}




