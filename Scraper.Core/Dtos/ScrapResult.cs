namespace Scraper.Core.Dtos;

public class ScrapResult
{
    public List<string> Values { get; set; } = new();
    public List<string> MatchedHtml { get; set; } = new();
    public string RawHtml { get; set; } = "";
}
