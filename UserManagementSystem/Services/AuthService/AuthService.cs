using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.DTOs;
using UserManagementSystem.Models.Identity;
using UserManagementSystem.Services.JwtService;

namespace UserManagementSystem.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequestDto request)
        {
            LoginResponse loginResponse = new LoginResponse();
            var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName == request.Username || u.Email == request.Username);

            if (user == null || !user.IsActive || user.Deleted)
            {
                throw new Exception("Invalid credentials or inactive user");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid credentials");
            }

            loginResponse.Token = _jwtService.GenerateToken(user);

            return loginResponse;

        }


        public async Task<ConfirmEmailResponseDto> ConfirmEmailAsync(string userId, string token)
        {

            ConfirmEmailResponseDto confirmEmailResponseDto = new ConfirmEmailResponseDto();
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                throw new Exception("Invalid request");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Invalid User");

            }
            if (user.EmailConfirmed)
            {
                throw new Exception("Email already confirmed");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                throw new Exception("Invalid or expired token");

            confirmEmailResponseDto.UserId = user.Id;
            confirmEmailResponseDto.SetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return confirmEmailResponseDto;

        }



        public async Task<bool> SetPasswordAsync(SetPasswordDto request)
        {
            bool passwordSet = false;
            // Find user by userId
            var user = await _userManager.FindByIdAsync(request.userID.ToString());
            if (user == null || !user.EmailConfirmed || !user.IsActive)
            {
                throw new Exception("Invalid token or user");
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!resetResult.Succeeded)
            {
                var errors = string.Join(",", resetResult.Errors.Select(e => e.Description));
                throw new Exception("Password reset failed: " + errors);
            }
            passwordSet = true;

            return passwordSet;

        }

    }
}
