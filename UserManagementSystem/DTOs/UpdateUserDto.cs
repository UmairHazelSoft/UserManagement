using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.DTOs
{
    
    public class UpdateUserDto
    {
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required]
        public bool? IsActive { get; set; } // optional
        public bool? TwoFactorEnabled { get; set; } // optional
    }

}
