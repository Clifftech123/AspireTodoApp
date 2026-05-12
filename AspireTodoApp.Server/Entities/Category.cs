namespace AspireTodoApp.Server.Entities;

public class Category : IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;
    public ICollection<Todo> Todos { get; set; } = [];

    // IAuditableEntity
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
