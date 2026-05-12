using AspireTodoApp.Server.Common;
using AspireTodoApp.Server.Data;
using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Features.Auth;
using AspireTodoApp.Server.Features.Todos.Dtos;
using AspireTodoApp.Server.Features.Validation;
using Microsoft.EntityFrameworkCore;

namespace AspireTodoApp.Server.Features.Todos;

public static class TodoEndpoints
{
    public static RouteGroupBuilder MapTodoEndpoints(this RouteGroupBuilder group)
    {
        group.RequireAuthorization("CurrentUser");

        group.MapGet("/", GetAll);
        group.MapGet("/deleted", GetDeleted);
        group.MapGet("/{id:guid}", GetById);

        group.MapPost("/", Create)
             .WithParameterValidation(typeof(CreateTodoRequest));

        group.MapPut("/{id:guid}", Update)
             .WithParameterValidation(typeof(UpdateTodoRequest));

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
        var result = await db.Todos
            .Where(t => t.UserId == currentUser.Id)
            .Include(t => t.Category)
            .OrderByDescending(t => t.CreatedAtUtc)
            .Select(t => t.ToResponse())
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
        var result = await db.Todos
            .IgnoreQueryFilters()
            .Where(t => t.UserId == currentUser.Id && t.IsDeleted)
            .Include(t => t.Category)
            .OrderByDescending(t => t.DeletedAtUtc)
            .Select(t => t.ToResponse())
            .ToPagedResultAsync(page, pageSize, ct);

        return Results.Ok(ApiResponse.Ok(result));
    }

    private static async Task<IResult> GetById(Guid id, CurrentUser currentUser, AppDbContext db, CancellationToken ct)
    {
        var todo = await db.Todos
            .Where(t => t.Id == id && t.UserId == currentUser.Id)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(ct);

        return todo is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse.Ok(todo.ToResponse()));
    }

    private static async Task<IResult> Create(
        CreateTodoRequest request,
        CurrentUser currentUser,
        AppDbContext db,
        CancellationToken ct)
    {
        var todo = new Todo
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            UserId = currentUser.Id,
        };

        db.Todos.Add(todo);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/todos/{todo.Id}", ApiResponse.Ok(todo.ToResponse()));
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateTodoRequest request,
        CurrentUser currentUser,
        AppDbContext db,
        CancellationToken ct)
    {
        var todo = await db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == currentUser.Id, ct);

        if (todo is null) return Results.NotFound();

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.CategoryId = request.CategoryId;

        if (request.IsCompleted && !todo.IsCompleted)
            todo.CompletedAt = DateTime.UtcNow;
        else if (!request.IsCompleted)
            todo.CompletedAt = null;

        todo.IsCompleted = request.IsCompleted;

        await db.SaveChangesAsync(ct);
        return Results.Ok(ApiResponse.Ok(todo.ToResponse()));
    }

    private static async Task<IResult> Delete(
        Guid id,
        CurrentUser currentUser,
        AppDbContext db,
        CancellationToken ct)
    {
        var todo = await db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == currentUser.Id, ct);

        if (todo is null) return Results.NotFound();

        db.Todos.Remove(todo);
        await db.SaveChangesAsync(ct);
        return Results.NoContent();
    }

    private static async Task<IResult> Restore(
        Guid id,
        CurrentUser currentUser,
        AppDbContext db,
        CancellationToken ct)
    {
        var todo = await db.Todos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == currentUser.Id && t.IsDeleted, ct);

        if (todo is null) return Results.NotFound();

        todo.IsDeleted = false;
        todo.DeletedAtUtc = null;
        todo.DeletedBy = null;

        await db.SaveChangesAsync(ct);
        return Results.Ok(ApiResponse.Ok(todo.ToResponse()));
    }
}
