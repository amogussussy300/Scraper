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
        PagingParams PagingParams,
        string? nameFilter = null,
        string? linkFilter = null
        )
    {

        await using var db = await _factory.CreateDbContextAsync();
        var query = db.Sources.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nameFilter)) { query = query.Where(s => s.Name.Contains(nameFilter)); }
        if (!string.IsNullOrWhiteSpace(linkFilter)) { query = query.Where(s => s.Link.Contains(linkFilter)); }

        var total = await query.CountAsync();

        if (PagingParams.SortDescending)
        {
            query = query.OrderByDescending(PagingParams.SortBy);
        }
        else
        {
            query = query.OrderBy(PagingParams.SortBy);
        }

        var items = await query
            .Skip((PagingParams.Page - 1) * PagingParams.PageSize)
            .Take(PagingParams.PageSize)
            .Select(s => new SourceDto { Id = s.Id, Name = s.Name, Link = s.Link })
            .ToListAsync();

        return new PaginatedResponse<SourceDto>
        {
            Items = items,
            TotalCount = total,
            Page = PagingParams.Page,
            PageSize = PagingParams.PageSize
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
