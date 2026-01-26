namespace UserManagementSystem.Helpers
{

    public static class AppSettings
    {
        private static IConfiguration _configuration;

        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Connection String
        public static string DefaultConnection => _configuration.GetConnectionString("DefaultConnection");

        // JWT Settings
        public static string JwtKey => _configuration["Jwt:Key"];
        public static string JwtIssuer => _configuration["Jwt:Issuer"];
        public static string JwtAudience => _configuration["Jwt:Audience"];
        public static int JwtDurationInMinutes => int.Parse(_configuration["Jwt:DurationInMinutes"] ?? "60");

        // Email Settings
        public static string SmtpHost => _configuration["Email:SmtpHost"];
        public static int SmtpPort => int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        public static string SmtpUser => _configuration["Email:SmtpUser"];
        public static string SmtpPass => _configuration["Email:SmtpPass"];
        public static string FromEmail => _configuration["Email:FromEmail"];

        // Confirm Email URL
        public static string ConfirmEmailUrl => _configuration["ConfirmEmailUrl"];
        // Email template
        public static string ConfirmEmailTemplate => _configuration["EmailTemplates:ConfirmEmail"];

    }


}
