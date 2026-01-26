using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Models.Identity;

namespace UserManagementSystem.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Optional: Override table name or add extra configurations
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename Identity tables (optional)
            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("AspNetUsers");
                b.Property(u => u.IsActive).HasDefaultValue(true);
                b.Property(u => u.Deleted).HasDefaultValue(false);
            });

            builder.Entity<ApplicationRole>(b =>
            {
                b.ToTable("AspNetRoles");
            });

            // Rename join tables
            //builder.Entity<IdentityUserRole<int>>().ToTable("AspNetUserRoles");
            //builder.Entity<IdentityUserClaim<int>>().ToTable("AspNetUserClaims");
            //builder.Entity<IdentityUserLogin<int>>().ToTable("AspNetUserLogins");
            //builder.Entity<IdentityRoleClaim<int>>().ToTable("AspNetRoleClaims");
            //builder.Entity<IdentityUserToken<int>>().ToTable("AspNetUserTokens");
        }
    }
}
