namespace Scraper.Core.Entities;

public class EntityType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public ICollection<EntityFieldType> Fields { get; set; } = new List<EntityFieldType>();
}
