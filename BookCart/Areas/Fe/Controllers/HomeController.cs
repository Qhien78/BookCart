using BookCart.Data;
using BookCart.Dto;
using BookCart.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookCart.Areas.fe.Controllers
{
    [Area("fe")]
    public class HomeController : Controller
    {
        // Key lưu chuỗi json của Cart
        public const string CARTKEY = "cart";

        readonly BookCartDbContext _ctx;

        public HomeController(BookCartDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            List<Book> featured = await _ctx.Books.Where(b=>b.Features != null && b.Features.Value).Take(2).ToListAsync();
            ViewBag.Featured = featured;
            List<Book> bestSelling = await _ctx.Books.Take(8).ToListAsync();
            List<Book> latest = await _ctx.Books.Take(8).ToListAsync();
            ViewBag.BestSelling = bestSelling;
            ViewBag.Latest = latest;
            return View();
        }

        // fe/home/bookdeails/id
        public async Task<IActionResult> BookDetails(int id)
        {
            Book? book = await _ctx.Books.SingleOrDefaultAsync(b => b.Id == id);
            return View(book);
        }

        // fe/home/books
        // princeRange: 10 - 30
        public async Task<IActionResult> Books(int? c, string? priceRange, string? author)
        {
            List<Book> books = await _ctx.Books.ToListAsync();
            var categories = await _ctx.Categories.ToListAsync();
            ViewBag.Categories = categories;
            

            List<decimal> prices = books
                .Select(b => b.Price!.Value)
                .OrderBy(b => b)
                .ToList();
            ViewBag.PriceMin = prices[0];
            ViewBag.PriceMax = prices[prices.Count-1];

            List<string> authors = books
                .Select(b => b.Author!)
                .Distinct()
                .ToList();

            ViewBag.Authors = authors;


            if (c != null)
            {
                books = await _ctx.Books
                    .Where(b => b.CategoryId == c)
                    .ToListAsync();
            }

            if (priceRange != null && author != null)
            {
                string[] priceList = priceRange.Split("-");
                decimal min = Convert.ToDecimal(priceList[0].Trim());
                decimal max = Convert.ToDecimal(priceList[1].Trim());
                books = books.Where(b => b.Price!.Value >= min && b.Price!.Value <= max && b.Author == author)
                    .ToList();
            }
            else if (priceRange != null)
            {
                string[] priceList = priceRange.Split("-");
                decimal min = Convert.ToDecimal(priceList[0].Trim());
                decimal max = Convert.ToDecimal(priceList[1].Trim());
                books = books.Where(b => b.Price!.Value >= min && b.Price!.Value <= max)
                    .ToList();
            }
            else if (author != null)
            {
                books = books.Where(b => b.Author == author)
                    .ToList();
            }


            return View(books);
        }

        

        // Lấy cart từ Session (danh sách CartItem)
        List<CartDto>? GetCartItems()
        {

            var session = HttpContext.Session;
            string? jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartDto>>(jsoncart);
            }
            return new List<CartDto>();
        }

        // Xóa cart khỏi session
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession(List<CartDto> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        //[HttpGet()]
        // /fe/home/addcart/id
        public async Task<IActionResult> AddCart(int id)
        {
            Book? book = await _ctx.Books.SingleOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound("Không tìm thấy sách");
            }

            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var cartitem = cart!.Find(p => p.Item!.Id == id);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.Quantity++;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartDto { Quantity = 1, Item = book });
            }

            // Lưu cart vào Session
            SaveCartSession(cart);

            // Lấy URL trước đó từ header Referer
            string refererUrl = Request.Headers["Referer"].ToString();

            // Nếu refererUrl không có, quay về trang chủ
            return Redirect(string.IsNullOrEmpty(refererUrl) ? "/fe" : refererUrl);
        }

        public IActionResult ViewCart()
        {
            return View(GetCartItems());
        }

        /// Cập nhật
        //[Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromBody] List<ViewCartItemDto> cartItems)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = GetCartItems();
            if (cart != null)
            {
                foreach (var item in cartItems)
                {
                    var cartitem = cart!.Find(p => p.Item!.Id == item.Id);
                    if (cartitem != null)
                    {
                        // Đã tồn tại, tăng thêm 1
                        cartitem.Quantity = item.Quantity;
                    }
                }

                SaveCartSession(cart);
            }
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }

        /// xóa item trong cart
        //[Route("/removecart/{id:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int id)
        {
            var cart = GetCartItems();
            if (cart != null)
            {
                var cartitem = cart.Find(p => p.Item!.Id == id);
                if (cartitem != null)
                {
                    // Đã tồn tại, tăng thêm 1
                    cart.Remove(cartitem);
                }

                SaveCartSession(cart);
            }
            return RedirectToAction(nameof(ViewCart));
        }

        public IActionResult ClearCart([FromRoute] int id)
        {
            var cart = GetCartItems();
            if (cart != null)
            {
                var cartitem = cart.Find(p => p.Item!.Id == id);
                if (cartitem != null)
                {
                    // Đã tồn tại, tăng thêm 1
                    cart.Remove(cartitem);
                }

                SaveCartSession(cart);
            }
            return RedirectToAction(nameof(ViewCart));
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
