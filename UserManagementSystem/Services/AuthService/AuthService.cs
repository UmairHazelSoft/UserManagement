using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UserManagementSystem.DTOs;
using UserManagementSystem.Models.Identity;
using UserManagementSystem.Services.JwtService;

namespace UserManagementSystem.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService; 
        private readonly IEmailSender _emailSender;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _emailSender = emailSender;
        }

        public async Task<ApiResponse<object>> LoginAsync(LoginRequestDto request)
        {
            try
            {
                //var user = await _userManager.FindByNameAsync(request.Username);

                var user = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.UserName == request.Username || u.Email == request.Username);

                if (user == null || !user.IsActive || user.Deleted)
                {
                    return new ApiResponse<object>(null, false, "Invalid credentials or inactive user", HttpStatusCode.Unauthorized);
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!isPasswordValid)
                {
                    return new ApiResponse<object>(null, false, "Invalid credentials", HttpStatusCode.Unauthorized);
                }

                var token = _jwtService.GenerateToken(user);

                return new ApiResponse<object>(new { token }, true, "Login successful", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(null, false, "An unexpected error occurred: " + ex.Message,HttpStatusCode.InternalServerError);
            }
        }


        public async Task<ApiResponse<object>> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                    return new ApiResponse<object>(null, false, "Invalid request");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new ApiResponse<object>(null, false, "Invalid user");

                var decodedToken = WebUtility.UrlDecode(token);

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                    return new ApiResponse<object>(null, false, "Invalid or expired token");

                // 🔐 Generate SET PASSWORD token
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                return new ApiResponse<object>(
                    new
                    {
                        UserId = user.Id,
                        SetPasswordToken = resetToken
                    },
                    true,
                    "Email confirmed. Use token to set password."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(null, false, "An unexpected error occurred: " + ex.Message, HttpStatusCode.InternalServerError);
            }

        }



        public async Task<ApiResponse<object>> SetPasswordAsync(SetPasswordDto request)
        {
            try
            {
            // Find user by userId
            var user = await _userManager.FindByIdAsync(request.userID.ToString());
            if (user == null || !user.EmailConfirmed || !user.IsActive)
                return new ApiResponse<object>(null, false, "Invalid token or user");

            // Reset password using Identity
            var resetResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!resetResult.Succeeded)
            {
                var errors = string.Join("; ", resetResult.Errors.Select(e => e.Description));
                return new ApiResponse<object>(null, false, errors);
            }

            return new ApiResponse<object>(null, true, "Password set successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(null, false, "An unexpected error occurred: " + ex.Message, HttpStatusCode.InternalServerError);
            }

        }

    }
}
