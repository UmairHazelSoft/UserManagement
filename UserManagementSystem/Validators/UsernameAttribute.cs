using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace UserManagementSystem.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UsernameAttribute : ValidationAttribute
    {
        private static readonly Regex _regex =
            new(@"^[a-zA-Z](?!.*__)[a-zA-Z0-9_]{1,18}[a-zA-Z0-9]$", RegexOptions.Compiled);

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            // Let [Required] handle null / empty
            if (value is null)
                return ValidationResult.Success;

            if (value is not string username)
                return new ValidationResult("Invalid username value.");

            if (!_regex.IsMatch(username))
                return new ValidationResult(
                    ErrorMessage ??
                    "Username must:\n" +
                    "- Be 3–20 characters long\n" +
                    "- Start with a letter (A–Z or a–z)\n" +
                    "- Contain only letters, digits, or underscores\n" +
                    "- Not have consecutive underscores\n" +
                    "- Not start or end with an underscore"
                );

            return ValidationResult.Success;
        }
    }
}
