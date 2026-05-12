using AspireTodoApp.Server.Features.Auth;
using AspireTodoApp.Server.Features.AuditTrails;
using Microsoft.AspNetCore.HttpLogging;

namespace AspireTodoApp.Server.Extensions;

public static class AppServicesExtensions
{
    public static IHostApplicationBuilder AddAppServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDataProtection(o => o.ApplicationDiscriminator = "AspireTodoApp");

        builder.Services.AddProblemDetails();
        builder.Services.AddRateLimiting();

        builder.Services.AddOpenApi(options => options.AddBearerTokenAuthentication());

        builder.Services.AddHttpLogging(o =>
        {
            if (builder.Environment.IsDevelopment())
            {
                o.CombineLogs = true;
                o.LoggingFields = HttpLoggingFields.RequestMethod
                                | HttpLoggingFields.RequestPath
                                | HttpLoggingFields.ResponseStatusCode
                                | HttpLoggingFields.Duration;
            }
        });

        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<IAuditService, AuditService>();

        return builder;
    }
}
