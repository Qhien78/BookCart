using BookCart.Models;

namespace BookCart.Dto
{
    public class CartDto
    {
        public Book? Item { get; set; }
        public int Quantity { get; set; }
    }
}
