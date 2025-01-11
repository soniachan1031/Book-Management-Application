using System.ComponentModel.DataAnnotations;

namespace Assignment3_ShongChan.Validations
{
    public class UsernameValidation : ValidationAttribute 
    {
        public UsernameValidation()
        {
            ErrorMessage = "Username must be between 5-20 characters.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var username = value?.ToString();

            if (string.IsNullOrEmpty(username))
            {
                return new ValidationResult("Username is required.");
            }

            if (username.Length < 5 || username.Length > 20)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
