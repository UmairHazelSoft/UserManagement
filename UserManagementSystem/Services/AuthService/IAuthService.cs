using UserManagementSystem.DTOs;

namespace UserManagementSystem.Services.AuthService
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequestDto request);
        Task<ConfirmEmailResponseDto> ConfirmEmailAsync(string userId, string token);
        Task<bool> SetPasswordAsync(SetPasswordDto request);
    }
}
