using GOLF.EVENT.Domains;
using Microsoft.EntityFrameworkCore;

namespace GOLF.EVENT.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<TrackPlayerPosition> TrackPlayerPositions { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Player>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TrackPlayerPosition>()
                .HasKey(t => t.Id);

        }
    }
}