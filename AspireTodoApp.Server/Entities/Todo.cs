namespace AspireTodoApp.Server.Entities;

public class Todo : IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;

    // IAuditableEntity
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
