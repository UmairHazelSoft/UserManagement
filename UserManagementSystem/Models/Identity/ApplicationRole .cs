using Microsoft.AspNetCore.Identity;

namespace UserManagementSystem.Models.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
