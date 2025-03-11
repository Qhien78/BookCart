using BookCart.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCart.Data
{
    public class BookCartDbContext : DbContext
    {
        public BookCartDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}
