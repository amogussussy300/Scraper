using Microsoft.EntityFrameworkCore;

namespace Scraper.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ItemEntity> Items => Set<ItemEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemEntity>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(200);
                b.Property(x => x.Link).HasMaxLength(2048);
                b.HasIndex(x => x.Link).IsUnique();
            });
        }
    }
}
