using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UserManagementSystem.DTOs;
using UserManagementSystem.Helpers;
using UserManagementSystem.Models.Identity;
using UserManagementSystem.Repositories.GenericRepository;

namespace UserManagementSystem.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        public UserService(IGenericRepository<ApplicationUser> userRepository,
                           IMapper mapper,
                           UserManager<ApplicationUser> userManager,
                           IEmailSender emailSender, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task<bool> CreateUserAsync(RegisterRequestDto request)
        {
            bool userCreated = false;
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already registered");
            }

            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                IsActive = true,
                Deleted = false
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception("User creation failed: " + errors);
            }

            userCreated = true;
            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var confirmUrl = $"{_configuration[AppSettings.ConfirmEmailUrl]}?userId={user.Id}&token={encodedToken}";

            // Get template from AppSettings and replace placeholder
            var emailBody = _configuration[AppSettings.ConfirmEmailTemplate].Replace("{ConfirmUrl}", confirmUrl);

            // _emailSender.SendEmailAsync(user.Email, _configuration[AppSettings.EmailHeader], emailBody);

            return userCreated;


        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            // Get user from Identity
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => !u.Deleted && u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            _mapper.Map(userDto, user);


            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                string errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new Exception("User update failed: " + errors);
            }

            var userResponse = _mapper.Map<UserResponseDto>(user);

            return userResponse;

        }


        public async Task<bool> DeleteUserAsync(int id)
        {
            bool deleted = false;
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => !u.Deleted && u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found or already deleted");
            }

            user.Deleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception("User deletion failed: " + errors);
            }
            return deleted = true;

        }



        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => !u.Deleted && u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var userDto = _mapper.Map<UserResponseDto>(user);

            return userDto;
        }

        public async Task<PagedResultDto<UserResponseDto>> GetPagedUsersAsync(GenericPaginationParams pagination)
        {
            
            var query = _userRepository.Query();

            var pagedUsers = await query.ApplyPagingAsync(pagination);

            return new PagedResultDto<UserResponseDto>
            {
                Items = _mapper.Map<IEnumerable<UserResponseDto>>(pagedUsers.Items),
                TotalCount = pagedUsers.TotalCount,
                PageNumber = pagedUsers.PageNumber,
                PageSize = pagedUsers.PageSize
            };
        }

    }
}
