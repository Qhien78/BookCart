using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCart.Models
{
    [Table("CartDetail")]
    public class CartDetail  // Đúng với tên bảng trong DB
    {
        [Key]  // Xác định Id là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng nếu cần
        public int Id { get; set; }

        [Required] // Đảm bảo không null
        public int CartId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }  // Sửa kiểu dữ liệu từ string -> decimal

        [Column(TypeName = "money")]
        public decimal PriceDiscount { get; set; }  // Sửa kiểu dữ liệu từ string -> decimal

        public int Quantity { get; set; }

        // Khóa ngoại
        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}
