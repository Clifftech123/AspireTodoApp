using AspireTodoApp.Server.Data;
using AspireTodoApp.Server.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AspireTodoApp.Server.Extensions;

public static class DatabaseExtensions
{
    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        // Interceptors — SoftDelete is singleton (stateless), Audit is scoped (holds pending entries per request)
        builder.Services.AddSingleton<SoftDeleteInterceptor>();
        builder.Services.AddScoped<AuditInterceptor>();

        builder.Services.AddSingleton<IInterceptor>(sp => sp.GetRequiredService<SoftDeleteInterceptor>());
        builder.Services.AddScoped<IInterceptor>(sp => sp.GetRequiredService<AuditInterceptor>());

        builder.AddNpgsqlDbContext<AppDbContext>("tododb");
        builder.AddRedisDistributedCache("cache");

        return builder;
    }
}
