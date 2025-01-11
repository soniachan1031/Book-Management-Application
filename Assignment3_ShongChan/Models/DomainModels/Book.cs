using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment3_ShongChan.Models.DomainModels
{
    public class Book
    {
        [Key]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author must be between 2 and 100 characters")]
        public string Author { get; set; } = string.Empty;

        [ForeignKey("GenreId")]
        [Required(ErrorMessage = "GenreId is required")]
        public int GenreId { get; set; }

        [ValidateNever]
        public Genre? Genre { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1000, 9999, ErrorMessage = "Year must be a valid four-digit year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be less than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "AvailableQuantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "AvailableQuantity cannot be less than 0")]
        public int AvailableQuantity { get; set; }

        [Required(ErrorMessage = "IsAvailable is required")]
        public bool IsAvailable { get; set; }

        [ForeignKey("BorrowedById")]
        public int? BorrowedById { get; set; }

        public User? BorrowedBy { get; set; }


        // Validation logic to ensure model consistency
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AvailableQuantity > Quantity)
            {
                yield return new ValidationResult(
                    "AvailableQuantity cannot exceed Quantity",
                    new[] { nameof(AvailableQuantity) }
                );
            }

            if (AvailableQuantity == 0 && IsAvailable)
            {
                yield return new ValidationResult(
                    "IsAvailable cannot be true while AvailableQuantity is 0",
                    new[] { nameof(IsAvailable) }
                );
            }

            if (AvailableQuantity > 0 && !IsAvailable)
            {
                yield return new ValidationResult(
                    "IsAvailable cannot be false while AvailableQuantity is greater than 0",
                    new[] { nameof(IsAvailable) }
                );
            }
        }
    }
}
