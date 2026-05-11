using Microsoft.EntityFrameworkCore;
using Scraper.Core.Data;
using Scraper.Core.Dtos;
using Scraper.Core.Entities;
using System.Linq;

namespace Scraper.Core.Services;

public class SourceService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public SourceService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public async Task<PaginatedResponse<SourceDto>> GetFilteredAsync(
        int page = 1,
        int pageSize = 10,
        string? nameFilter = null,
        string? linkFilter = null,
        string sortBy = "Name",
        bool sortDescending = false)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var query = db.Sources.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nameFilter)) { query = query.Where(s => s.Name.Contains(nameFilter)); }
        if (!string.IsNullOrWhiteSpace(linkFilter)) { query = query.Where(s => s.Link.Contains(linkFilter)); }

        var total = await query.CountAsync();

        if (sortDescending)
        {
            query = query.OrderByDescending(sortBy);
        }
        else
        {
            query = query.OrderBy(sortBy);
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SourceDto { Id = s.Id, Name = s.Name, Link = s.Link })
            .ToListAsync();

        return new PaginatedResponse<SourceDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
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
