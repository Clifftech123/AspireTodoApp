using AspireTodoApp.Server.Extensions;
using AspireTodoApp.Server.Features.AuditTrails;
using AspireTodoApp.Server.Features.Auth;
using AspireTodoApp.Server.Features.Categories;
using AspireTodoApp.Server.Features.Todos;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDatabase();
builder.AddAuth();
builder.AddAppServices();

var app = builder.Build();

await app.MigrateDatabaseAsync();

app.UseExceptionHandler();
app.UseHttpLogging();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Aspire Todo API";
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = ["Bearer"]
        };
    });

    app.Map("/", () => Results.Redirect("/scalar/v1"));
}

var api = app.MapGroup("/api");

api.MapGroup("/auth").MapAuthEndpoints();
api.MapGroup("/todos").MapTodoEndpoints().RequirePerUserRateLimit();
api.MapGroup("/categories").MapCategoryEndpoints().RequirePerUserRateLimit();
api.MapGroup("/audit-logs").MapAuditTrailEndpoints().RequirePerUserRateLimit();

app.MapDefaultEndpoints();
app.UseFileServer();

app.Run();
