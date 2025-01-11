using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Assignment3_ShongChan.Validations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Assignment3_ShongChan.Models.DomainModels
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [UsernameValidation]
        public string Username { get; set; } = string.Empty;

        [PasswordValidation]
        public string? Password { get; set; } = string.Empty;

        [NotMapped]
        [ConfirmPasswordValidation("Password")]
        public string? ConfirmPassword { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Age { get; set; }

        [DateOfBirthValidation]
        public DateTime DateOfBirth { get; set; }

        [RoleValidation]
        public string Role { get; set; } = "Member";

        public string? ProfilePic { get; set; } = "/Images/default-profile.png";

        // Foreign key for ProfileImage
        public int? ProfileImageId { get; set; }

        // Navigation property for the Image
        public virtual Image? ProfileImage { get; set; }

        public int? BookId { get; set; }

        // Navigation property for associated book
        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        // Foreign key property for the issued book
        public int? IssuedBookId { get; set; }

        [ForeignKey("IssuedBookId")] // Navigation property for the book issued by the user
        [ValidateNever]
        public Book? IssuedBook { get; set; }

        [InverseProperty("BorrowedBy")]  // Navigation property for the books borrowed by the user
        public ICollection<Book> IssuedBooks { get; set; } = new List<Book>();

        [NotMapped]
        public string AssignedBook => Book != null ? Book.Title : "No Book Assigned"; // Display the assigned book title
    }
}
