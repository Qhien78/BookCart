using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCart.Models
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Name { get; set; }
        public List<User>? Users
        {
            get; set;
        }
    }
}
