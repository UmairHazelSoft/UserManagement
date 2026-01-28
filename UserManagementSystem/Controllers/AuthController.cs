using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Controllers.Base;
using UserManagementSystem.DTOs;
using UserManagementSystem.Services.AuthService;

namespace UserManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
            return HandleResponse(response); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            return HandleResponse(response); 
        }



        
    }
}
