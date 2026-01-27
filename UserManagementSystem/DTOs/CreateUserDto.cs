using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

    }
}
