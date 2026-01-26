using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementSystem.Controllers.Base;
using UserManagementSystem.DTOs;
using UserManagementSystem.Models.Identity;
using UserManagementSystem.Services.AuthService;

namespace UserManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);
            return HandleResponse(response); // one line
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, string token)
        {
            var response = await _authService.ConfirmEmailAsync(userId,token);
            return HandleResponse(response);
        }


        [HttpPost("setpassword")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto request)
        {
            var response = await _authService.SetPasswordAsync(request);
            return HandleResponse(response); // one line
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // Call AuthService to handle login logic
            var response = await _authService.LoginAsync(request);
            return HandleResponse(response); // one line
        }



        
    }
}
