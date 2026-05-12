using AspireTodoApp.Server.Data;
using AspireTodoApp.Server.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AspireTodoApp.Server.Extensions;

public static class DatabaseExtensions
{
    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSingleton<SoftDeleteInterceptor>();
        builder.Services.AddSingleton<AuditInterceptor>();

        builder.Services.AddSingleton<IDbContextOptionsConfiguration<AppDbContext>>(sp =>
            new InterceptorOptionsConfiguration(
                sp.GetRequiredService<SoftDeleteInterceptor>(),
                sp.GetRequiredService<AuditInterceptor>()));

        builder.AddNpgsqlDbContext<AppDbContext>("tododb");
        builder.AddRedisDistributedCache("cache");

        return builder;
    }

    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    private sealed class InterceptorOptionsConfiguration(
        SoftDeleteInterceptor softDelete,
        AuditInterceptor audit) : IDbContextOptionsConfiguration<AppDbContext>
    {
        public void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            builder.AddInterceptors(softDelete, audit);
        }
    }
}
