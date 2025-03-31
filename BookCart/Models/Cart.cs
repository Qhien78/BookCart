using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCart.Models
{
    [Table("Carts")]
    public class Cart
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? Fullname { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string? Phone { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string? Address { get; set; }
        [Column(TypeName = "Timestamp")]
        public DateTime DateAt { get; set; } = DateTime.Now;
        [ForeignKey("UserId")]
        public User? User { set; get; }

        public List<CartDetail>? CartDetails { set; get; }
    }
}
