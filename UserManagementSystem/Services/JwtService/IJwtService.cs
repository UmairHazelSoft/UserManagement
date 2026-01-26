using UserManagementSystem.Models.Identity;

namespace UserManagementSystem.Services.JwtService
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user);
    }
}
