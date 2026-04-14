using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Index on Email for fast lookup
        builder.Entity<AppUser>()
            .HasIndex(u => u.Email)
            .HasDatabaseName("IX_Users_Email");

        // Index on LastLoginAt for sorting
        builder.Entity<AppUser>()
            .HasIndex(u => u.LastLoginAt)
            .HasDatabaseName("IX_Users_LastLoginAt");

        // Index on IsBlocked for filtering
        builder.Entity<AppUser>()
            .HasIndex(u => u.IsBlocked)
            .HasDatabaseName("IX_Users_IsBlocked");
    }
}
