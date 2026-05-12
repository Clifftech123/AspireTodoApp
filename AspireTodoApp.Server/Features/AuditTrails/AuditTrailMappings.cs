using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Features.AuditTrails.Dtos;

namespace AspireTodoApp.Server.Features.AuditTrails;

public static class AuditTrailMappings
{
    public static AuditTrailDto ToDto(this AuditTrail trail) => new(
        trail.Id,
        trail.UserId,
        trail.User?.UserName,
        trail.TrailType,
        trail.DateUtc,
        trail.EntityName,
        trail.PrimaryKey,
        trail.OldValues,
        trail.NewValues,
        trail.ChangedColumns);
}
