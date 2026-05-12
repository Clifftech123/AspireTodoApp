using AspireTodoApp.Server.Common;
using AspireTodoApp.Server.Data;
using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Enums;
using AspireTodoApp.Server.Features.AuditTrails.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AspireTodoApp.Server.Features.AuditTrails;

public class AuditService(AppDbContext db) : IAuditService
{
    public async Task LogAsync(
        string? userId,
        TrailType trailType,
        string entityName,
        string? primaryKey = null,
        Dictionary<string, object?>? oldValues = null,
        Dictionary<string, object?>? newValues = null,
        List<string>? changedColumns = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityName);

        var trail = new AuditTrail
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TrailType = trailType,
            DateUtc = DateTime.UtcNow,
            EntityName = entityName,
            PrimaryKey = primaryKey,
            OldValues = oldValues ?? [],
            NewValues = newValues ?? [],
            ChangedColumns = changedColumns ?? []
        };

        db.AuditTrails.Add(trail);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<AuditTrailDto>> GetLogsAsync(
        string? userId,
        TrailType? trailType,
        string? entityName,
        DateTime? fromUtc,
        DateTime? toUtc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = db.AuditTrails
            .AsNoTracking()
            .Include(t => t.User)
            .AsQueryable();

        if (userId is not null)
            query = query.Where(t => t.UserId == userId);

        if (trailType.HasValue)
            query = query.Where(t => t.TrailType == trailType.Value);

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(t => EF.Functions.ILike(t.EntityName, $"%{entityName}%"));

        if (fromUtc.HasValue)
            query = query.Where(t => t.DateUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(t => t.DateUtc <= toUtc.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.DateUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditTrailDto>
        {
            Items = items.Select(t => t.ToDto()).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<int> ClearOldLogsAsync(int days = 15, CancellationToken cancellationToken = default)
    {
        if (days < 1)
            throw new ArgumentOutOfRangeException(nameof(days), "Days must be at least 1.");

        var threshold = DateTime.UtcNow.AddDays(-days);

        return await db.AuditTrails
            .Where(t => t.DateUtc < threshold)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
