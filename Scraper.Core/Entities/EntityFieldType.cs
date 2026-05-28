using Scraper.Core.Enums;

namespace Scraper.Core.Entities;

public class EntityFieldType
{
    public int Id { get; set; }
    public string FieldName { get; set; } = "";
    public FieldType FieldType { get; set; }

    public int EntityTypeId { get; set; }
    public EntityType EntityType { get; set; } = default!;
}
