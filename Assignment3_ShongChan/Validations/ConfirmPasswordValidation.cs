using Assignment3_ShongChan.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Assignment3_ShongChan.Validations
{
    public class ConfirmPasswordValidation : ValidationAttribute
    {
        private readonly string _passwordPropertyName;
        public ConfirmPasswordValidation(string passwordPropertyName)
        {
            _passwordPropertyName = passwordPropertyName; 
            ErrorMessage = "Password and Confirm Passwords do not match."; 
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) 
            {
                return new ValidationResult("Confirm Password is required.");
            }

            var instance = validationContext.ObjectInstance; // Get the instance of the object that is being validated
            var passwordProperty = validationContext.ObjectType.GetProperty(_passwordPropertyName); // Get the property of the object that is being validated

            if (passwordProperty == null)
            {
                return new ValidationResult($"Property '{_passwordPropertyName}' not found."); 
            }
               
            var passwordValue = passwordProperty.GetValue(instance)?.ToString(); // Get the value of the property
            var confirmPasswordValue = value?.ToString(); // Get the value of the ConfirmPassword property

            if (passwordValue != confirmPasswordValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
