using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scraper.Core.Entities;

namespace Scraper.Core.Data;

public class ScrapDetailConfiguration : IEntityTypeConfiguration<ScrapDetail>
{
    public void Configure(EntityTypeBuilder<ScrapDetail> builder)
    {
        builder.ToTable("ScrapDetails");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.Target).HasMaxLength(200);

        builder.Property(s => s.ActionType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(s => s.Source)
            .WithMany()
            .HasForeignKey(s => s.SourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Actions)
            .WithOne(a => a.ScrapDetail)
            .HasForeignKey(a => a.ScrapDetailId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}