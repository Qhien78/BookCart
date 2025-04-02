namespace BookCart.Models
{
    public class PaymentInformation
    {
        public string? Fullname { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
