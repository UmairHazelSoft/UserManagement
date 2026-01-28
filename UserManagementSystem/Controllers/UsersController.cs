using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Controllers.Base;
using UserManagementSystem.DTOs;
using UserManagementSystem.Services.UserService;

namespace UserManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GenericPaginationParams pagination)
        {
            var response = await _userService.GetPagedUsersAsync(pagination);
            return HandleResponse(response);
        }

        [HttpGet("GetUserById/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return HandleResponse(response);
        }


        [HttpPut("UpdateUser/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            var response = await _userService.UpdateUserAsync(id, userDto);
            return HandleResponse(response); 
        }


        [HttpDelete("DeleteUser/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userService.DeleteUserAsync(id);
            return HandleResponse(response); 

        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDto request)
        {
            var response = await _userService.CreateUser(request);
            return HandleResponse(response);
        }

    }
}
