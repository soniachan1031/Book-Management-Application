using System.ComponentModel.DataAnnotations;

namespace Assignment3_ShongChan.Models.DomainModels
{
    public class Genre
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be within 2 to 100 characters")]
        public string? Name { get; set; }
    }
}
