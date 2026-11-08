using System.Collections.Concurrent;
using System.Security.Claims;

namespace SmartShop.Admin.Security;

// Revocation also ends administrator access from other open Blazor tabs.
public sealed class AdminSessions
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> sessions = new();

    public string Create(DateTimeOffset expires)
    {
        foreach (var entry in sessions)
            if (entry.Value <= DateTimeOffset.UtcNow) sessions.TryRemove(entry.Key, out _);
        string id = Guid.NewGuid().ToString("N");
        sessions[id] = expires;
        return id;
    }

    public bool IsValid(ClaimsPrincipal principal)
    {
        string? id = principal.FindFirst("admin_session")?.Value;
        return principal.Identity?.IsAuthenticated == true && principal.IsInRole("Admin")
            && id is not null && sessions.TryGetValue(id, out var expires)
            && expires > DateTimeOffset.UtcNow;
    }

    public void Revoke(ClaimsPrincipal principal)
    {
        string? id = principal.FindFirst("admin_session")?.Value;
        if (id is not null) sessions.TryRemove(id, out _);
    }
}
