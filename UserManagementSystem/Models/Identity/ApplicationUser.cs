using Microsoft.AspNetCore.Identity;

namespace UserManagementSystem.Models.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public int RoleId { get; set; }
        public ApplicationRole Role { get; set; }  // Navigation property
    }
}
