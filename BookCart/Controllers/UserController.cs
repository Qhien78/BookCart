using BookCart.Data;
using BookCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BookCart.Controllers
{
    public class UserController : Controller
    {
        readonly BookCartDbContext _ctx;

        public UserController(BookCartDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _ctx.Users.ToListAsync();
            return View(users);
        }
        public async IActionResult Create()
        {
            var roles = await _ctx.Roles.ToListAsync();
            ViewBag.Roles = roles;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                await _ctx.Users.AddAsync(user);
                await _ctx.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }
    }
}
