using System.ComponentModel.DataAnnotations;

namespace Assignment3_ShongChan.Models.DomainModels
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // MIME type (e.g., "image/jpeg")
        [Required]
        public required string ContentType { get; set; } = string.Empty;


        // Path to the image file in the folder
        [Required]
        public required string ImagePath { get; set; } = string.Empty;
    }
}
