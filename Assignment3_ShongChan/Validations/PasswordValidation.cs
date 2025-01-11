using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Assignment3_ShongChan.Validations
{
    public class PasswordValidation : ValidationAttribute
    {
        public PasswordValidation()
        {
            ErrorMessage = "Password must have 8+ characters, an uppercase letter, number, and special character.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value?.ToString();

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required.");
            }

            if (password.Length < 8)
            {
                return new ValidationResult("Password must be at least 8+ characters long.");
            }

            if (!password.Any(char.IsUpper))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!password.Any(char.IsDigit))
            {
                return new ValidationResult("Password must contain at least one number.");
            }

            var specialCharacters = "!@#$%^&*()_+-=[]{}|;:',.<>?/`~";
            if (!password.Any(c => specialCharacters.Contains(c)))
            {
                return new ValidationResult("Password must contain at least one special character.");
            }

            return ValidationResult.Success;
        }
    }
}
