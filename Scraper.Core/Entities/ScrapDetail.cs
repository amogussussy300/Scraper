using Scraper.Core.Enums;

namespace Scraper.Core.Entities;

public class ScrapDetail
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public Source Source { get; set; } = default!;

    public ActionType ActionType { get; set; } = ActionType.ExtractField;
    public string Target { get; set; } = "";

    public ICollection<ScrapAction> Actions { get; set; } = new List<ScrapAction>();
}