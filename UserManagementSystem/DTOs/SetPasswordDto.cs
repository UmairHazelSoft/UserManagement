namespace UserManagementSystem.DTOs
{
    public class SetPasswordDto
    {
        public int userID { get; set; }
        public string Token { get; set; }  // Password reset token generated after email confirm
        public string NewPassword { get; set; }
    }
}
