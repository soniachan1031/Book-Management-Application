using System.ComponentModel.DataAnnotations;
using System.Xml.Schema;

namespace Assignment3_ShongChan.Validations
{
    public class RoleValidation : ValidationAttribute
    {
        private static readonly string[] _roles = { "Member", "Admin"};

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var role = value?.ToString();
            if (string.IsNullOrEmpty(role))
            {
                return new ValidationResult("Role is required.");
            }
            if (!string.IsNullOrEmpty(role) && !_roles.Contains(role))
            {
                return new ValidationResult("Role must be either 'Member' or 'Admin'.");
            }

            return ValidationResult.Success;
        }
    }
}
