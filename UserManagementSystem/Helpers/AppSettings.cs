using System.Reflection.Metadata;

namespace UserManagementSystem.Helpers
{

    public static class AppSettings
    {
        

        // Connection String
        public const string DefaultConnection = "DefaultConnection";

        // JWT Settings
        public const string JwtKey ="Jwt:Key";
        public const string JwtIssuer ="Jwt:Issuer";
        public const string JwtAudience ="Jwt:Audience";
        public const string JwtDurationInMinutes = "Jwt:DurationInMinutes";

        // Email Settings
        public const string SmtpHost ="Email:SmtpHost";
        public const string SmtpPort = "Email:SmtpPort";
        public const string SmtpUser ="Email:SmtpUser";
        public const string SmtpPass ="Email:SmtpPass";
        public const string FromEmail ="Email:FromEmail";

        // Confirm Email URL
        public static string ConfirmEmailUrl ="ConfirmEmailUrl";
        // Email template
        public const string ConfirmEmailTemplate ="EmailTemplates:ConfirmEmail";

        public const string EmailHeader = "Confirm your email";
        public const string UnAuthoriized = "Unauthorized - Authentication required";
        public const string Forbidden = "Forbidden - You do not have permission to access this resource";

        public const string ResourceNotFound = "Resource not found";
        public const string ErrorOccurred = "An unexpected server error occurred";

        public const string Deleted = "Deleted";

    }


}
