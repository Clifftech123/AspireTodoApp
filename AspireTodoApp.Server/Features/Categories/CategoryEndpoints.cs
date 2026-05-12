using AspireTodoApp.Server.Common;
using AspireTodoApp.Server.Data;
using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Features.Auth;
using AspireTodoApp.Server.Features.Categories.Dtos;
using AspireTodoApp.Server.Features.Validation;
using Microsoft.EntityFrameworkCore;

namespace AspireTodoApp.Server.Features.Categories;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this RouteGroupBuilder group)
    {
        group.RequireAuthorization("CurrentUser");

        group.MapGet("/", GetAll);
        group.MapGet("/deleted", GetDeleted);
        group.MapGet("/{id:guid}", GetById);

        group.MapPost("/", Create)
             .WithParameterValidation(typeof(CreateCategoryRequest));

        group.MapPut("/{id:guid}", Update)
             .WithParameterValidation(typeof(UpdateCategoryRequest));

        group.MapDelete("/{id:guid}", Delete);
        group.MapPut("/{id:guid}/restore", Restore);

        return group;
    }

    private static async Task<IResult> GetAll(
        CurrentUser currentUser,
        AppDbContext db,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await db.Categories
            .Where(c => c.UserId == currentUser.Id)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Todos.Count))
            .ToPagedResultAsync(page, pageSize, ct);

        return Results.Ok(ApiResponse.Ok(result));
    }

    private static async Task<IResult> GetDeleted(
        CurrentUser currentUser,
        AppDbContext db,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await db.Categories
            .IgnoreQueryFilters()
            .Where(c => c.UserId == currentUser.Id && c.IsDeleted)
            .OrderByDescending(c => c.DeletedAtUtc)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Todos.Count))
            .ToPagedResultAsync(page, pageSize, ct);

        return Results.Ok(ApiResponse.Ok(result));
    }

    private static async Task<IResult> GetById(Guid id, CurrentUser currentUser, AppDbContext db, CancellationToken ct)
    {
        var category = await db.Categories
            .Where(c => c.Id == id && c.UserId == currentUser.Id)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Todos.Count))
            .FirstOrDefaultAsync(ct);

        return category is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse.Ok(category));
    }

    private static async Task<IResult> Create(
        CreateCategoryRequest request,
        CurrentUser currentUser,
        AppDbContext db,
        CancellationToken ct)
    {
        var category = new Category
        {
            Name = request.Name,
            UserId = currentUser.Id,
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/categories/{category.Id}",
            ApiResponse.Ok(new CategoryResponse(category.Id, category.Name, 0)));
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateCategoryRequest request,
        CurrentUser currentUser,
        AppDbContext db,
        CancellationToken ct)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == currentUser.Id, ct);

        if (category is null) return Results.NotFound();

        category.Name = request.Name;

        await db.SaveChangesAsync(ct);

        var todoCount = await db.Todos.CountAsync(t => t.CategoryId == id, ct);
        return Results.Ok(ApiResponse.Ok(new CategoryResponse(category.Id, category.Name, todoCount)));
    }

    private static async Task<IResult> Delete(
        Guid id,
        CurrentUser currentUser,
        AppDbContext db,
        CancellationToken ct)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == currentUser.Id, ct);

        if (category is null) return Results.NotFound();

        db.Categories.Remove(category);
        await db.SaveChangesAsync(ct);
        return Results.NoContent();
    }

    private static async Task<IResult> Restore(
        Guid id,
        CurrentUser currentUser,
        AppDbContext db,
        CancellationToken ct)
    {
        var category = await db.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == currentUser.Id && c.IsDeleted, ct);

        if (category is null) return Results.NotFound();

        category.IsDeleted = false;
        category.DeletedAtUtc = null;
        category.DeletedBy = null;

        await db.SaveChangesAsync(ct);
        return Results.Ok(ApiResponse.Ok(category.ToResponse()));
    }
}
