using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
