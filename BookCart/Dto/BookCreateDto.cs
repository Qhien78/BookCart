
using System.ComponentModel.DataAnnotations;

namespace BookCart.Dto
{
    public class BookCreateDto
    {
        [Required]
        [StringLength(200)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal PriceDiscount { get; set; }
        public IFormFile? Image { get; set; }
    }
}
