using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scraper.Core.Entities;

namespace Scraper.Core.Data;

public class ScrapActionConfiguration : IEntityTypeConfiguration<ScrapAction>
{
    public void Configure(EntityTypeBuilder<ScrapAction> builder)
    {
        builder.ToTable("ScrapActions");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.Selector).HasMaxLength(2000);
    }
}