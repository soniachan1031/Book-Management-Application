using System.ComponentModel.DataAnnotations;

namespace Assignment3_ShongChan.Validations
{
    public class DateOfBirthValidation : ValidationAttribute
    {
        public DateOfBirthValidation()
        {
            ErrorMessage = "The date of birth cannot be today or in the future.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dateOfBirth = value as System.DateTime?;

            if(dateOfBirth == null || dateOfBirth == DateTime.MinValue)
            {
                return new ValidationResult("Please select a valid date of birth(MM-DD-YYYY).");
            }

            if (dateOfBirth >= System.DateTime.Now )
            {
                return new ValidationResult(ErrorMessage);
            }


            return ValidationResult.Success;
        }
    }
}
