using AspireTodoApp.Server.Common;
using AspireTodoApp.Server.Enums;
using AspireTodoApp.Server.Features.AuditTrails.Dtos;

namespace AspireTodoApp.Server.Features.AuditTrails;

public interface IAuditService
{
    Task LogAsync(
        string? userId,
        TrailType trailType,
        string entityName,
        string? primaryKey = null,
        Dictionary<string, object?>? oldValues = null,
        Dictionary<string, object?>? newValues = null,
        List<string>? changedColumns = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AuditTrailDto>> GetLogsAsync(
        string? userId,
        TrailType? trailType,
        string? entityName,
        DateTime? fromUtc,
        DateTime? toUtc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> ClearOldLogsAsync(int days = 15, CancellationToken cancellationToken = default);
}
