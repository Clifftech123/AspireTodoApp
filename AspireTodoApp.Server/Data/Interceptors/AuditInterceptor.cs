using System.Security.Claims;
using AspireTodoApp.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AspireTodoApp.Server.Data.Interceptors;

public class AuditInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    private static readonly AsyncLocal<List<AuditEntry>?> _pendingEntries = new();

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            _pendingEntries.Value = CaptureEntries(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var entries = _pendingEntries.Value;

        if (eventData.Context is not null && entries?.Count > 0)
        {
            var userId = httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var trails = entries.Select(e => e.ToAuditTrail(userId)).ToList();

            _pendingEntries.Value = null;

            eventData.Context.Set<AuditTrail>().AddRange(trails);
            await eventData.Context.SaveChangesAsync(cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static List<AuditEntry> CaptureEntries(DbContext context)
    {
        context.ChangeTracker.DetectChanges();

        return context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is not AuditTrail
                     && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => new AuditEntry(e))
            .ToList();
    }
}
