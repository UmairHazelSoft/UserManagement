using UserManagementSystem.DTOs;

namespace UserManagementSystem.Services.UserService
{
    public interface IUserService
    {

        Task<bool> CreateUserAsync(RegisterRequestDto request);

        Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto userDto);
        Task<bool> DeleteUserAsync(int id);

        Task<UserResponseDto> GetUserByIdAsync(int id) ;
        Task<PagedResultDto<UserResponseDto>> GetPagedUsersAsync(GenericPaginationParams pagination);
    }
}
