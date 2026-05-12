using AspireTodoApp.Server.Common;
using AspireTodoApp.Server.Enums;
using AspireTodoApp.Server.Features.Auth;

namespace AspireTodoApp.Server.Features.AuditTrails;

public static class AuditTrailEndpoints
{
    public static RouteGroupBuilder MapAuditTrailEndpoints(this RouteGroupBuilder group)
    {
        group.RequireAuthorization("CurrentUser");

        group.MapGet("/", GetLogs);
        group.MapDelete("/clear", ClearOldLogs);

        return group;
    }

    private static async Task<IResult> GetLogs(
        CurrentUser currentUser,
        IAuditService auditService,
        TrailType? trailType,
        string? entityName,
        DateTime? fromUtc,
        DateTime? toUtc,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        if (pageSize > 100) pageSize = 100;

        var result = await auditService.GetLogsAsync(
            currentUser.Id,
            trailType,
            entityName,
            fromUtc,
            toUtc,
            page,
            pageSize,
            ct);

        return Results.Ok(ApiResponse.Ok(result));
    }

    private static async Task<IResult> ClearOldLogs(
        IAuditService auditService,
        int days = 30,
        CancellationToken ct = default)
    {
        var deleted = await auditService.ClearOldLogsAsync(days, ct);
        return Results.Ok(ApiResponse.Ok($"Cleared {deleted} log entries older than {days} days."));
    }
}
