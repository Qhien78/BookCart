using BookCart.Models;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.Data.SqlClient;
using PayPal.v1.Invoices;
using PayPal.v1.Payments;
using System.Net;
using System.Transactions;

namespace BookCart.Service
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _configuration;
        private const double ExchangeRate = 22_863.0;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public static double ConvertVndToDollar(decimal vnd)
        {
            var total = Math.Round((double)vnd / ExchangeRate, 2);
        }

        public async Task<string> CreatePaymentUrl(PaymentInformation model, HttpContext context)
        {
            var envSandBox = new SandboxEnvironment(_configuration["PayPal:ClientId"], _configuration["PayPal:SecretKey"]);
            var client = new PayPalHttpClient(envSandBox);
            var paypalOrderId = DateTime.Now.Ticks;
            var urlCallBack = _configuration["PayPal:ReturnUrl"];
            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = ConvertVndToDollar(model.Amount).ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Subtotal = ConvertVndToDollar(model.Amount).ToString(),
                                Tax = "0",
                                Shipping = "0",
                            }
                        },
                        ItemList = new ItemList()
                        {
                            Items = new List<Item>()
                            {
                                Name = "| Order: " + model.Description,
                                Currency = "USD",
                                Price = ConvertVndToDollar(model.Amount).ToString(),
                                Quantity = 1.ToString(),
                                Sku = "sku",
                                Tax = "0",
                                url = "https://localhost:5001/fe/home/cart",
                            }
                        }
                    },
                    Description = $"Payment for {model.Description}",
                },
                RedirectUrls = new RedirectUrls()
                {
                    ReturnUrl = $"{urlCallBack}?payment_method=PayPal&success=1&order_id={paypalOrderId}",
                    CancelUrl = $"{urlCallBack}?payment_method=PayPal&success=0&order_id={paypalOrderId}",
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };
            var request = new PaymentCreateRequest();
            request.RequestBody(payment);
            var PaymentUrl = "";
            var response = await client.Execute(request);
            var statusCode = response.StatusCode;
            if (statusCode is not (HttpStatusCode.Accepted or HttpStatusCode.OK or HttpStatusCode.Created))
                return paymentUrl;

            var result = response.Result<Payment>();
            using var links = result.Links.GetEnumerator();
            while (links.MoveNext())
            {
                var lnk = links.Current;
                if (lnk == null) continue;
                if(!lnk.Rel.ToLower().Trim().Equals("approval_url")) continue;
                PaymentUrl = lnk.Href;
                }
            return PaymentUrl;
            }

        public PaymentResponse PaymentExecute(IQueryCollection collections)
        {
            var response = new PaymentResponse();
            foreach(var(key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("order_description"))
                {
                    response.OrderDescription = value;
                }
                if(!string.IsNullOrEmpty(key) && key.ToLower().Equals("transaction_id"))
                {
                    response.TransactionId = value;
                }
                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("order_id"))
                {
                    response.OrderId = value;
                }
                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("payment_method"))
                {
                    response.PaymentMethod = value;
                }
                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("success"))
                {
                    response.Success = Convert.ToInt32(value) > 0;
                }
                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("paymentid"))
                {
                    response.PaymentId = value;
                }
                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("payerid"))
                {
                    response.PayerId = value;
                }
            }
        }
    }
}
