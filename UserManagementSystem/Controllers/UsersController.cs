using AutoMapper;
using Azure.Core;
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

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GenericPaginationParams pagination)
        {
            try
            {
                var response = await _userService.GetPagedUsersAsync(pagination);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUserById/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var response = await _userService.GetUserByIdAsync(id);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("UpdateUser/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            try
            {
                var response = await _userService.UpdateUserAsync(id, userDto);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("DeleteUser/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var response = await _userService.DeleteUserAsync(id);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDto request)
        {
            try
            {
                var response = await _userService.CreateUserAsync(request);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
