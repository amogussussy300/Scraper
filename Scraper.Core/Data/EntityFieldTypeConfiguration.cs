using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scraper.Core.Entities;

namespace Scraper.Core.Data;

public class EntityFieldTypeConfiguration : IEntityTypeConfiguration<EntityFieldType>
{
    public void Configure(EntityTypeBuilder<EntityFieldType> builder)
    {
        builder.ToTable("EntityFieldTypes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.FieldName).HasMaxLength(200);

        builder.Property(e => e.FieldType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(e => e.EntityType)
            .WithMany(et => et.Fields)
            .HasForeignKey(e => e.EntityTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}