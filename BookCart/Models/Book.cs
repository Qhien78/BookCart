using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCart.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        //[Column(TypeName = "text")]
        public string? Description { get; set; }
        [Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Column(TypeName = "money")]
        public decimal? PriceDiscount { get; set; }
        public string? Image { get; set; }
        public bool? Features { set; get; }
        public string? Author { set; get; }

        public int? CategoryId { set; get; }
        [ForeignKey("CategoryId")]
        public Category? Category { set; get; }
    }
}
