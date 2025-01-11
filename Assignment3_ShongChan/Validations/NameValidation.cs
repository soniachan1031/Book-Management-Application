using System.ComponentModel.DataAnnotations;

namespace Assignment3_ShongChan.Validations
{
    public class NameValidation : ValidationAttribute
    {
        public NameValidation()
        {
            ErrorMessage = "Name must be between 2-50 characters.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var name = value as string;

            if (string.IsNullOrEmpty(name))
            {
                return new ValidationResult("Name is required.");
            }

            if (name.Length < 2 || name.Length > 50)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
