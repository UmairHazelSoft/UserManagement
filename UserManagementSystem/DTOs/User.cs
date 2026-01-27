using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.DTOs
{
    public class User
    {
        /// <summary>
        /// Primary key of the Users table.
        /// </summary>
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Password { get; set; } = string.Empty;
    }
}
