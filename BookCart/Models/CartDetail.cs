using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCart.Models
{
    [Table("CartDetail")]
    public class CartDetail
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        public int CartId { get; set; }
        public int BookId { get; set; }
        [Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Column(TypeName = "money")]
        public decimal? PriceDiscount { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("CartId")]
        public Cart? Cart { set; get; }
        [ForeignKey("BookId")]
        public Book? Book { set; get; }
    }
}
