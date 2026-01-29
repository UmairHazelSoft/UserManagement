using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.DTOs
{
    public class ConfirmEmailResponseDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string SetPasswordToken  { get; set; }
    }
}
