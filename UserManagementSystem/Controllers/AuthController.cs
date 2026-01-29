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


        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, string token)
        {
            try
            {
                var response = await _authService.ConfirmEmailAsync(userId, token);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("SetPassword")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto request)
        {
            try
            {
                var response = await _authService.SetPasswordAsync(request);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        
    }
}
