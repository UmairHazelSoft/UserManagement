using System;
using System.ComponentModel.DataAnnotations;
namespace UserManagementSystem.CustomValidators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CustomEmailAttribute : ValidationAttribute
    {
        // Optional: default error message
        private const string DefaultErrorMessage = "Invalid email format. Email must contain exactly one '@', a valid domain, and up to 3 TLD parts.";

        public CustomEmailAttribute()
        {
            ErrorMessage = DefaultErrorMessage;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Allow nulls here; [Required] handles mandatory check
            if (value is null)
                return ValidationResult.Success;

            // Ensure the value is a string
            if (value is not string email)
                return new ValidationResult("Email value must be a string.");

            // Split by '@' to ensure exactly one @
            var parts = email.Split('@');
            if (parts.Length != 2)
                return new ValidationResult("Email must contain exactly one '@' character.");

            var usernamePart = parts[0];
            var domainPart = parts[1];

            // Basic username validation: cannot be empty or whitespace
            if (string.IsNullOrWhiteSpace(usernamePart))
                return new ValidationResult("Email username part cannot be empty.");

            // Split domain by dot '.'
            var domainParts = domainPart.Split('.');
            if (domainParts.Length < 2)
                return new ValidationResult("Email domain must have at least one TLD (e.g., .com).");

            if (domainParts.Length > 4) // 1–3 TLDs allowed + main domain
                return new ValidationResult("Email domain can have up to 3 parts after the main domain (e.g., .com, .edu.pk, .com.net).");

            // Optional: check that each part only contains letters, numbers, hyphens
            foreach (var part in domainParts)
            {
                if (string.IsNullOrWhiteSpace(part) || !IsValidDomainPart(part))
                    return new ValidationResult($"Invalid domain part '{part}' in email.");
            }

            return ValidationResult.Success;
        }

        private bool IsValidDomainPart(string part)
        {
            foreach (char c in part)
            {
                if (!char.IsLetterOrDigit(c) && c != '-')
                    return false;
            }
            return true;
        }
    }
}


