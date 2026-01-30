using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementSystem.Helpers;
using UserManagementSystem.Models.Identity;
namespace UserManagementSystem.Services.JwtService
{
    public class JwtService : IJwtService
    {

        private  IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(ApplicationUser user)
        {
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[AppSettings.JwtKey]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Add claims
            var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };

            var token = new JwtSecurityToken(
                issuer: _configuration[AppSettings.JwtIssuer] ,
                audience: _configuration[AppSettings.JwtAudience] ,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration[AppSettings.JwtDurationInMinutes] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
