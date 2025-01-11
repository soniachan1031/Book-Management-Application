using System.ComponentModel.DataAnnotations;
using Assignment3_ShongChan.Validations;

namespace Assignment3_ShongChan.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [PasswordValidation]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; } 


        [ConfirmPasswordValidation("NewPassword")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; } 
    }
}
