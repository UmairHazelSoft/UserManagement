using UserManagementSystem.DTOs;

namespace UserManagementSystem.Services.UserService
{
    public interface IUserService
    {

        Task<ApiResponse<object>> CreateUser(RegisterRequestDto request);

        Task<UserReadDto> CreateUserAsync(CreateUserDto userDto);
        Task<ApiResponse<UserResponseDto>> UpdateUserAsync(int id, UpdateUserDto userDto);
        Task<ApiResponse<UserResponseDto>> DeleteUserAsync(int id);

        //Task<bool> DeleteUserAsync(int id);
        Task<ApiResponse<UserResponseDto>> GetUserByIdAsync(int id) ;
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
        //Task<PagedResult<UserReadDto>> GetPagedUsersAsync(PaginationParams pagination);
        //Task<ApiResponse<PagedResult<UserResponseDto>>> GetPagedUsersAsync();
        Task<ApiResponse<PagedResult<UserResponseDto>>> GetPagedUsersAsync(GenericPaginationParams pagination);
    }
}
