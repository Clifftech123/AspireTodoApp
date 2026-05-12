using Microsoft.AspNetCore.Identity;

namespace AspireTodoApp.Server.Entities;

public class AppUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public ICollection<Todo> Todos { get; set; } = [];
    public ICollection<Category> Categories { get; set; } = [];

    public string? FirstName =>
        DisplayName?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

    public string? LastName =>
        DisplayName?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).LastOrDefault();

    public void SetDisplayName(string? displayName) =>
        DisplayName = displayName?.Trim();
}
