using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.DTOs
{
    //public class UpdateUserDto
    //{
    //    [Required]
    //    public string Name { get; set; }

    //    [Required, EmailAddress]
    //    public string Email { get; set; }


    //}
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public bool? IsActive { get; set; } // optional
        public bool? TwoFactorEnabled { get; set; } // optional
    }

}
