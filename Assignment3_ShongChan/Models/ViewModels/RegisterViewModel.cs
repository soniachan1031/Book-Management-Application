using System.ComponentModel.DataAnnotations;
using Assignment3_ShongChan.Models;
using Assignment3_ShongChan.Models.DomainModels;
using Assignment3_ShongChan.Validations;

namespace Assignment3_ShongChan.Models.ViewModels
{
    public class RegisterViewModel
    {
        // Add Id property for updating members
        public int? Id { get; set; }

        [UsernameValidation]
        public string Username { get; set; } = string.Empty;

        [PasswordValidation]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [ConfirmPasswordValidation("Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [NameValidation]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Age { get; set; } = string.Empty;

        [DateOfBirthValidation]
        public DateTime? DateOfBirth { get; set; }

        public IFormFile? ProfileImage { get; set; }
        public string? ProfilePic { get; set; }
    }
}
