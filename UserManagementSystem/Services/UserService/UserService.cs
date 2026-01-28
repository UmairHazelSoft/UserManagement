using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UserManagementSystem.DTOs;
using UserManagementSystem.Helpers;
using UserManagementSystem.Models.Identity;
using UserManagementSystem.Repositories;

namespace UserManagementSystem.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public UserService(IRepository<ApplicationUser> userRepository,
                           IMapper mapper,
                           UserManager<ApplicationUser> userManager,
                           IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<ApiResponse<object>> CreateUserAsync(RegisterRequestDto request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                    return new ApiResponse<object>(null, false, "Email already registered");

                var user = new ApplicationUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    IsActive = true,
                    Deleted = false
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return new ApiResponse<object>(null, false, string.Join("; ", result.Errors.Select(e => e.Description)));

                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = Uri.EscapeDataString(token);


                var confirmUrl = $"{AppSettings.ConfirmEmailUrl}?userId={user.Id}&token={encodedToken}";

                // Get template from AppSettings and replace placeholder
                var emailBody = AppSettings.ConfirmEmailTemplate.Replace("{ConfirmUrl}", confirmUrl);

                 _emailSender.SendEmailAsync(
                    user.Email,
                    AppSettings.EmailHeader,
                    emailBody
                );
                
                return new ApiResponse<object>(null, true, "User registered. Check email to confirm.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(null, false, "An unexpected error occurred: " + ex.Message, HttpStatusCode.InternalServerError);
            }

        }


        public async Task<ApiResponse<UserResponseDto>> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            try
            {
                // Get user from Identity
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => !u.Deleted && u.Id == id);

                if (user == null)
                    return new ApiResponse<UserResponseDto>(null, false, "User not found", HttpStatusCode.NotFound);

                // Check if username is taken by another user
                var usernameInUse = await _userManager.Users
                    .AnyAsync(u => u.UserName.ToLower() == userDto.UserName.ToLower() && u.Id != user.Id);

                if (usernameInUse)
                    return new ApiResponse<UserResponseDto>(null, false, "Username is already in use", HttpStatusCode.BadRequest);

                // Check if email is taken by another user
                var emailInUse = await _userManager.Users
                    .AnyAsync(u => u.Email.ToLower() == userDto.Email.ToLower() && u.Id != user.Id);

                if (emailInUse)
                    return new ApiResponse<UserResponseDto>(null, false, "Email is already in use", HttpStatusCode.BadRequest);

                // Map only allowed fields
                _mapper.Map(userDto, user);

              
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                    return new ApiResponse<UserResponseDto>(null, false,
                        string.Join("; ", updateResult.Errors.Select(e => e.Description)),
                        HttpStatusCode.InternalServerError);

                var userResponse = _mapper.Map<UserResponseDto>(user);

                return new ApiResponse<UserResponseDto>(userResponse, true, "User updated successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserResponseDto>(null, false, "An error occurred on the server side:", HttpStatusCode.InternalServerError);
            }
        }


        public async Task<ApiResponse<UserResponseDto>> DeleteUserAsync(int id)
        {
            try
            {
                
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => !u.Deleted && u.Id == id);

                if (user == null)
                {
                    return new ApiResponse<UserResponseDto>(
                        null,
                        false,
                        "User not found or already deleted",
                        HttpStatusCode.NotFound
                    );
                }

                user.Deleted = true;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return new ApiResponse<UserResponseDto>(
                        null,
                        false,
                        string.Join("; ", result.Errors.Select(e => e.Description)),
                        HttpStatusCode.InternalServerError
                    );
                }

                // Map to response DTO
                var userResponse = _mapper.Map<UserResponseDto>(user);

                return new ApiResponse<UserResponseDto>(
                    userResponse,
                    true,
                    "User deleted successfully",
                    HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserResponseDto>(
                    null,
                    false,
                    "An error occurred on the server side: " + ex.Message,
                    HttpStatusCode.InternalServerError
                );
            }
        }


        public async Task<ApiResponse<UserResponseDto>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => !u.Deleted && u.Id == id);

                if (user == null)
                {
                    return new ApiResponse<UserResponseDto>(
                        null,
                        false,
                        "User not found",
                        HttpStatusCode.NotFound
                    );
                }

                var userDto = _mapper.Map<UserResponseDto>(user);

                return new ApiResponse<UserResponseDto>(
                    userDto,
                    true,
                    "User fetched successfully",
                    HttpStatusCode.OK
                );
            }
            catch (Exception)
            {
                // log exception here (ILogger)
                return new ApiResponse<UserResponseDto>(
                    null,
                    false,
                    "An unexpected error occurred",
                    HttpStatusCode.InternalServerError
                );
            }
        }


        public async Task<ApiResponse<PagedResult<UserResponseDto>>> GetPagedUsersAsync(GenericPaginationParams pagination)
        {

            try
            {
               
                var result = await _userRepository.GetPagedAsync(
                            pagination,
                            user => _mapper.Map<UserResponseDto>(user));

                return new ApiResponse<PagedResult<UserResponseDto>>(result, true, "Users fetched successfully", HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<UserResponseDto>>(
                    null,
                    false,
                    "An unexpected error occurred",
                    HttpStatusCode.InternalServerError
                );
            }      

        }
    }
}
