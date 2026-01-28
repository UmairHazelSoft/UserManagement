using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementSystem.Helpers;
using UserManagementSystem.Models;
using UserManagementSystem.Models.Identity;
namespace UserManagementSystem.Services.JwtService
{
    public class JwtService : IJwtService
    {
        

        public JwtService()
        {
            
        }

        public string GenerateToken(ApplicationUser user)
        {
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Add claims
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: AppSettings.JwtIssuer ,
                audience: AppSettings.JwtAudience ,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(AppSettings.JwtDurationInMinutes)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
