using Microsoft.EntityFrameworkCore;
using Scraper.Core.Entities;

namespace Scraper.Core.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Source> Sources => Set<Source>();
    public DbSet<EntityType> EntityTypes => Set<EntityType>();
    public DbSet<EntityFieldType> EntityFieldTypes => Set<EntityFieldType>();
    public DbSet<ScrapDetail> ScrapDetails => Set<ScrapDetail>();
    public DbSet<ScrapAction> ScrapActions => Set<ScrapAction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
