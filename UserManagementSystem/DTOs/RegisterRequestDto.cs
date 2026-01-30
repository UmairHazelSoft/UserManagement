using System.ComponentModel.DataAnnotations;
using UserManagementSystem.CustomValidators;
using UserManagementSystem.Validators;

namespace UserManagementSystem.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [Username]
        public string Username { get; set; }

        [Required]
        [CustomEmail]
        public string Email { get; set; }
    }
}
