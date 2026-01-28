using UserManagementSystem.DTOs;

namespace UserManagementSystem.Services.UserService
{
    public interface IUserService
    {

        Task<ApiResponse<object>> CreateUser(RegisterRequestDto request);

        Task<ApiResponse<UserResponseDto>> UpdateUserAsync(int id, UpdateUserDto userDto);
        Task<ApiResponse<UserResponseDto>> DeleteUserAsync(int id);

        Task<ApiResponse<UserResponseDto>> GetUserByIdAsync(int id) ;
        Task<ApiResponse<PagedResult<UserResponseDto>>> GetPagedUsersAsync(GenericPaginationParams pagination);
    }
}
