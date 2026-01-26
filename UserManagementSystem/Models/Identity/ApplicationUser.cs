using Microsoft.AspNetCore.Identity;

namespace UserManagementSystem.Models.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public bool IsActive { get; set; } = true;
        public bool Deleted { get; set; } = false;
    }
}
