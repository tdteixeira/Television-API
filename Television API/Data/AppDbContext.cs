using Microsoft.EntityFrameworkCore;
using Television_API.Models;

namespace Television_API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Actor> Actors { get; set; }
        public DbSet<TVShow> TVShows { get; set; }
        public DbSet<Episode> Episodes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-Many: TVShow <-> Actor
            modelBuilder.Entity<TVShow>()
                .HasMany(t => t.actors)
                .WithMany(a => a.tvShows)
                .UsingEntity<Dictionary<string, object>>(
                    "TVShowActor",
                    j => j.HasOne<Actor>().WithMany().HasForeignKey("ActorId"),
                    j => j.HasOne<TVShow>().WithMany().HasForeignKey("TVShowId"));

            // One-to-Many: TVShow -> Episodes
            modelBuilder.Entity<TVShow>()
                .HasMany(t => t.episodes)
                .WithOne(e => e.tvShow)
                .HasForeignKey(e => e.tvShowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
