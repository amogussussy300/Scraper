using Microsoft.EntityFrameworkCore;
using Scraper.Core.Data;
using Scraper.Core.Dtos;
using Scraper.Core.Entities;
using Scraper.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Core.Services
{
    public class ScrapDetailService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;
        public ScrapDetailService(IDbContextFactory<AppDbContext> factory) => _factory = factory;
        public async Task<List<ScrapDetailDto>> GetBySourceAsync(int sourceId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.ScrapDetails
                .AsNoTracking()
                .Where(d => d.SourceId == sourceId)
                .OrderBy(d => d.Id)
                .Select(d => new ScrapDetailDto
                {
                    Id = d.Id,
                    SourceId = d.SourceId,
                    ActionType = d.ActionType,
                    Target = d.Target
                })
                .ToListAsync();
        }
        public async Task<ScrapDetailDto> CreateAsync(int sourceId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var entity = new ScrapDetail
            {
                SourceId = sourceId,
                ActionType = ActionType.ExtractField,
                Target = ""
            };
            db.ScrapDetails.Add(entity);
            await db.SaveChangesAsync();
            return new ScrapDetailDto
            {
                Id = entity.Id,
                SourceId = entity.SourceId,
                ActionType = entity.ActionType,
                Target = entity.Target
            };
        }
        public async Task<ScrapDetailDto?> GetWithActionsAsync(int detailId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var entity = await db.ScrapDetails
                .AsNoTracking()
                .Include(d => d.Actions)
                .FirstOrDefaultAsync(d => d.Id == detailId);
            if (entity == null) return null;
            return new ScrapDetailDto
            {
                Id = entity.Id,
                SourceId = entity.SourceId,
                ActionType = entity.ActionType,
                Target = entity.Target,
                Actions = entity.Actions
                    .OrderBy(a => a.Order)
                    .Select(a => new ScrapActionDto
                    {
                        Id = a.Id,
                        ScrapDetailId = a.ScrapDetailId,
                        Selector = a.Selector,
                        Order = a.Order
                    })
                    .ToList()
            };
        }
        public async Task SaveAsync(int detailId, string target, List<ScrapActionDto> actions)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var detail = await db.ScrapDetails
                .Include(d => d.Actions)
                .FirstOrDefaultAsync(d => d.Id == detailId);
            if (detail == null) return;
            detail.Target = target;
            // Стратегия replace-all: проще, чем диффить;
            // ScrapAction'ов мало (десятки максимум).
            db.ScrapActions.RemoveRange(detail.Actions);
            for (int i = 0; i < actions.Count; i++)
            {
                db.ScrapActions.Add(new ScrapAction
                {
                    ScrapDetailId = detailId,
                    Selector = actions[i].Selector,
                    Order = i
                });
            }
            await db.SaveChangesAsync();
        }
        public async Task DeleteAsync(int detailId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var detail = await db.ScrapDetails.FirstOrDefaultAsync(d => d.Id == detailId);
            if (detail == null) return;
            db.ScrapDetails.Remove(detail);
            await db.SaveChangesAsync();
        }
    }
}
