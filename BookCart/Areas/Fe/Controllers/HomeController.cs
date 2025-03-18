using BookCart.Data;
using BookCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCart.Areas.Fe.Controllers
{
    [Area("Fe")]
    public class HomeController : Controller
    {
        readonly BookCartDbContext _ctx;
        public HomeController(BookCartDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<IActionResult> Index()
        {
          List<Book> featured = await _ctx.Books.Where(b=>b.Features != null && b.Features.Value).Take(2).ToListAsync();
            ViewBag.Feature = featured;
            List<Book> bestSelling =await _ctx.Books.Take(8).ToListAsync();
            List<Book> latest =await _ctx.Books.Take(8).ToListAsync();
            ViewBag.BestSelling = bestSelling;
            ViewBag.Latest = latest;

            return View();
        }
    }
}
