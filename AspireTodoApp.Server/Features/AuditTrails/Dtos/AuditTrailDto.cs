using AspireTodoApp.Server.Enums;

namespace AspireTodoApp.Server.Features.AuditTrails.Dtos;

public record AuditTrailDto(
    Guid Id,
    string? UserId,
    string? UserName,
    TrailType TrailType,
    DateTime DateUtc,
    string EntityName,
    string? PrimaryKey,
    Dictionary<string, object?> OldValues,
    Dictionary<string, object?> NewValues,
    List<string> ChangedColumns);
