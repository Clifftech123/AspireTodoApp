using AspireTodoApp.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AspireTodoApp.Server.Data.Interceptors;

public class SoftDeleteInterceptor(IHttpContextAccessor? httpContextAccessor = null) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return ValueTask.FromResult(result);

        var userName = httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

        foreach (var entry in eventData.Context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State != EntityState.Deleted) continue;

            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAtUtc = DateTime.UtcNow;
            entry.Entity.DeletedBy = userName;

            CascadeSoftDelete(eventData.Context, entry.Entity, userName);
        }

        return ValueTask.FromResult(result);
    }

    private static void CascadeSoftDelete(DbContext context, ISoftDeletable parentEntity, string deletedBy)
    {
        foreach (var navigation in context.Entry(parentEntity).Navigations)
        {
            if (navigation.CurrentValue is null) continue;

            if (navigation.CurrentValue is IEnumerable<ISoftDeletable> children)
            {
                foreach (var child in children)
                {
                    if (child.IsDeleted) continue;
                    child.IsDeleted = true;
                    child.DeletedAtUtc = DateTime.UtcNow;
                    child.DeletedBy = deletedBy;
                    context.Entry(child).State = EntityState.Modified;
                }
            }
            else if (navigation.CurrentValue is ISoftDeletable child && !child.IsDeleted)
            {
                child.IsDeleted = true;
                child.DeletedAtUtc = DateTime.UtcNow;
                child.DeletedBy = deletedBy;
                context.Entry(child).State = EntityState.Modified;
            }
        }
    }
}
