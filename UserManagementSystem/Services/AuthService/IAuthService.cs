using UserManagementSystem.DTOs;

namespace UserManagementSystem.Services.AuthService
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<object>> ConfirmEmailAsync(string userId, string token);
        Task<ApiResponse<object>> SetPasswordAsync(SetPasswordDto request);
    }
}
