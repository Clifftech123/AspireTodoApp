using System.Security.Claims;
using AspireTodoApp.Server.Entities;

namespace AspireTodoApp.Server.Features.Auth;

public class CurrentUser
{
    public AppUser? User { get; set; }
    public ClaimsPrincipal Principal { get; set; } = default!;

    public string Id => Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
