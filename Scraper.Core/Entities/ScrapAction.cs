namespace Scraper.Core.Entities;

public class ScrapAction
{
    public int Id { get; set; }

    public int ScrapDetailId { get; set; }
    public ScrapDetail ScrapDetail { get; set; } = default!;

    public string Selector { get; set; } = "";
    public int Order { get; set; }
}