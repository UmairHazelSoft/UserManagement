using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.DTOs
{
    public class SetPasswordDto
    {
        [Required]
        public int userID { get; set; }
        [Required]
        public string Token { get; set; }  
        [Required]
        public string NewPassword { get; set; }
    }
}
