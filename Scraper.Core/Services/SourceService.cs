using Microsoft.EntityFrameworkCore;
using Scraper.Core.Data;
using Scraper.Core.Dtos;
using Scraper.Core.Entities;

namespace Scraper.Core.Services;

public class SourceService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public SourceService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public async Task<List<SourceDto>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Sources
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Select(s => new SourceDto { Id = s.Id, Name = s.Name, Link = s.Link })
            .ToListAsync();
    }

    public async Task<SourceDto> AddAsync(SourceDto dto)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entity = new Source { Name = dto.Name, Link = dto.Link };
        db.Sources.Add(entity);
        await db.SaveChangesAsync();
        return new SourceDto { Id = entity.Id, Name = entity.Name, Link = entity.Link };
    }

}
