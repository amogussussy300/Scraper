using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Scraper.Core.Dtos;
using Scraper.Core.Entities;

namespace Scraper.Core.Services;

public class HtmlParserService
{
    public ScrapResult Parse(string html, IEnumerable<ScrapAction> actions)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var ordered = actions
            .Where(a => !string.IsNullOrWhiteSpace(a.Selector))
            .OrderBy(a => a.Order)
            .ToList();

        var result = new ScrapResult { RawHtml = html };

        if (ordered.Count == 0)
            return result;

        IEnumerable<HtmlNode> currentNodes = new[] { doc.DocumentNode };

        foreach (var action in ordered)
        {
            var next = new List<HtmlNode>();
            foreach (var node in currentNodes)
            {
                try
                {
                    var found = node.QuerySelectorAll(action.Selector);
                    if (found != null)
                        next.AddRange(found);
                }
                catch
                {

                }
            }

            if (next.Count == 0)
            {

                return result;
            }

            currentNodes = next;
        }

        result.Values = currentNodes
            .Select(n => HtmlEntity.DeEntitize(n.InnerText ?? "").Trim())
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList();

        result.MatchedHtml = currentNodes
            .Select(n => n.OuterHtml)
            .ToList();

        return result;
    }
}
