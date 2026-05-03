using Microsoft.EntityFrameworkCore;
using Scraper.Data;

namespace Scraper.Services
{
    public class ItemService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;
        public ItemService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

        public async Task<List<ItemEntity>> GetAllAsync(CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.Items.AsNoTracking().OrderBy(i => i.Id).ToListAsync(ct);
        }

        public async Task<ItemEntity?> AddAsync(string name, string link, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var entity = new ItemEntity { Name = name, Link = link };
            db.Items.Add(entity);
            try
            {
                await db.SaveChangesAsync(ct);
                return entity;
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }
    }
}
