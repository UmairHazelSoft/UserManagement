using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        public string Username { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}
