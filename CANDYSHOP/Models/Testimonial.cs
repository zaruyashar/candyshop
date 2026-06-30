using System.ComponentModel.DataAnnotations;

namespace CANDYSHOP.Models
{
    public class Testimonial
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ClientName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Role { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [StringLength(250)]
        public string ImagePath { get; set; } = string.Empty;
    }
}
