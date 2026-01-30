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
                b.Property(u => u.IsDeleted).HasDefaultValue(false);

                b.HasIndex(u => u.UserName)
                    .IsUnique();

                b.HasIndex(u => u.Email)
                    .IsUnique();

                
                b.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict); 
            });

            builder.Entity<ApplicationRole>(b =>
            {
                b.ToTable("AspNetRoles");

                // Seed default roles
                b.HasData(
                    new ApplicationRole { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                    new ApplicationRole { Id = 2, Name = "User", NormalizedName = "USER" }
                );
            });

        }
    }
}
