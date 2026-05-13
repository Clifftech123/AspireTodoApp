using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AspireTodoApp.Server.Data.Interceptors;

public class AuditEntry(EntityEntry entry)
{
    // Eagerly captured during SavingChangesAsync, before EF Core calls AcceptAllChanges()
    // which resets entity states to Unchanged before SavedChangesAsync is invoked.
    public string EntityName { get; } = entry.Metadata.ClrType.Name;

    public string? PrimaryKey { get; } = entry.Properties
        .Where(p => p.Metadata.IsPrimaryKey())
        .Select(p => p.CurrentValue?.ToString())
        .FirstOrDefault();

    public TrailType TrailType { get; } = entry.State switch
    {
        EntityState.Added    => TrailType.Create,
        EntityState.Modified => TrailType.Update,
        EntityState.Deleted  => TrailType.Delete,
        _                    => TrailType.None
    };

    public Dictionary<string, object?> OldValues { get; } = entry.State == EntityState.Added
        ? []
        : entry.Properties
            .Where(p => p.IsModified || entry.State == EntityState.Deleted)
            .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);

    public Dictionary<string, object?> NewValues { get; } = entry.State == EntityState.Deleted
        ? []
        : entry.Properties
            .Where(p => p.IsModified || entry.State == EntityState.Added)
            .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

    public List<string> ChangedColumns { get; } = entry.State != EntityState.Modified
        ? []
        : entry.Properties
            .Where(p => p.IsModified)
            .Select(p => p.Metadata.Name)
            .ToList();

    public AuditTrail ToAuditTrail(string? userId) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        TrailType = TrailType,
        DateUtc = DateTime.UtcNow,
        EntityName = EntityName,
        PrimaryKey = PrimaryKey,
        OldValues = OldValues,
        NewValues = NewValues,
        ChangedColumns = ChangedColumns
    };
}
