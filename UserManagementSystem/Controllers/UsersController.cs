using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Controllers.Base;
using UserManagementSystem.DTOs;
using UserManagementSystem.Services.UserService;

namespace UserManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    
    //[Authorize]
    
    
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers(
            //[FromQuery] PaginationParams pagination
            [FromQuery] GenericPaginationParams pagination
            )
        {
            //var pagedResult = await _userService.GetPagedUsersAsync(pagination);
            var response = await _userService.GetPagedUsersAsync(pagination);

            //return Ok(new ApiResponse<PagedResult<UserReadDto>>(
            //    true,
            //    "Users fetched successfully",
            //    pagedResult
            //));
            return HandleResponse(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return HandleResponse(response);
        }

        //[HttpPost("create")]
        //public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        //{
        //    var createdUser = await _userService.CreateUserAsync(dto);
        //    var response = _mapper.Map<UserResponseDto>(createdUser);
        //    return Ok(new ApiResponse<UserResponseDto>(true, "User created successfully", response));
        //}

        //[HttpPut("update/{id:int}")]
        //public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        //{
        //    var updatedUser = await _userService.UpdateUserAsync(id, dto);
        //    if (updatedUser == null) return NotFound(new ApiResponse<string>(false, "User not found", null));

        //    var response = _mapper.Map<UserResponseDto>(updatedUser);
        //    return Ok(new ApiResponse<UserResponseDto>(true, "User updated successfully", response));
        //}
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            var response = await _userService.UpdateUserAsync(id, userDto);
            return HandleResponse(response); // Using your base controller
        }


        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userService.DeleteUserAsync(id);
            return HandleResponse(response); // Using your base controller

        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] RegisterRequestDto request)
        {
            var response = await _userService.CreateUser(request);
            return HandleResponse(response);
        }

    }
}
