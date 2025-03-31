using BookCart.Data;
using BookCart.Dto;
using BookCart.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookCart.Controllers
{
    public class BookController : Controller
    {
        const string IMG_FOLDER = "images";

        readonly BookCartDbContext _ctx;
        readonly IWebHostEnvironment _env;

        public BookController(BookCartDbContext ctx, IWebHostEnvironment env)
        {
            _ctx = ctx;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookCreateDto book)
        {
            if (ModelState.IsValid)
            {
                Book item = new Book
                {
                    Title = book.Title,
                    Description = book.Description,
                    Price = book.Price,
                    PriceDiscount = book.PriceDiscount
                };

                // xử lý file upload
                if (book.Image != null && book.Image.Length > 0)
                {
                    string imgFolder = Path.Combine(_env.WebRootPath, IMG_FOLDER);
                    if (!Directory.Exists(imgFolder))
                    {
                        Directory.CreateDirectory(imgFolder);
                    }

                    string imgPath = Path.Combine(imgFolder, book.Image.FileName);
                    using (var stream = new FileStream(imgPath, FileMode.Create))
                    {
                        await book.Image.CopyToAsync(stream);
                        item.Image = book.Image.FileName;
                    }
                }

                _ctx.Books.Add(item);
                await _ctx.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(book);
        }
    }
}
