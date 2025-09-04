using Microsoft.EntityFrameworkCore;
using Television_API.Models;

namespace Television_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<TVShow> TVShows { get; set; }
    }
}
