namespace UserManagementSystem.DTOs
{
    public class ConfirmEmailDto
    {
        public string Email { get; set; } // Optional, can remove if token encodes user
        public string Token { get; set; }
    }
}
