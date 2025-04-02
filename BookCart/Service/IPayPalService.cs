using PayPal.v1.Payments;
using BookCart.Models;

namespace BookCart.Service
{
    public interface IPayPalService
    {
        Task<string> CreatePaymentUrl(PaymentInformation model, HttpContext context);
        PaymentResponse PaymentExecute(IQueryCollection collections);
    }
}
