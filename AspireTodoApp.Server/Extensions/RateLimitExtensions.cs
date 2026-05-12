using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace AspireTodoApp.Server.Extensions;

public static class RateLimitExtensions
{
    private const string Policy = "PerUserRateLimit";

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter();

        services.AddOptions<TokenBucketRateLimiterOptions>()
            .Configure(options =>
            {
                options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                options.AutoReplenishment = true;
                options.TokenLimit = 100;
                options.TokensPerPeriod = 100;
                options.QueueLimit = 100;
            })
            .BindConfiguration("RateLimiting");

        services.AddOptions<RateLimiterOptions>()
            .Configure((RateLimiterOptions options, IOptionsMonitor<TokenBucketRateLimiterOptions> rateLimitOptions) =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(Policy, context =>
                {
                    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? context.Connection.RemoteIpAddress?.ToString()
                                 ?? "anonymous";

                    return RateLimitPartition.GetTokenBucketLimiter(userId, _ => rateLimitOptions.CurrentValue);
                });
            });

        return services;
    }

    public static IEndpointConventionBuilder RequirePerUserRateLimit(this IEndpointConventionBuilder builder)
        => builder.RequireRateLimiting(Policy);
}
