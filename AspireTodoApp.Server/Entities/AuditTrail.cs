using AspireTodoApp.Server.Enums;

namespace AspireTodoApp.Server.Entities;

public class AuditTrail
{
    public required Guid Id { get; set; }
    public string? UserId { get; set; }
    public AppUser? User { get; set; }
    public TrailType TrailType { get; set; }
    public DateTime DateUtc { get; set; }
    public required string EntityName { get; set; }
    public string? PrimaryKey { get; set; }
    public Dictionary<string, object?> OldValues { get; set; } = [];
    public Dictionary<string, object?> NewValues { get; set; } = [];
    public List<string> ChangedColumns { get; set; } = [];
}
