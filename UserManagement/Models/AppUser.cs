using Microsoft.AspNetCore.Identity;

namespace UserManagement.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsBlocked { get; set; } = false;
}
