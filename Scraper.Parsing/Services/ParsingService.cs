using HtmlAgilityPack;
namespace Scraper.Parsing.Services;

public class ParsingService
{
    // results = [body]; foreach(rule) results = ApplyRule(results, rule)
    public IReadOnlyList<Element> Extract(string html, IEnumerable<string> cssSelectors)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var body = doc.DocumentNode.SelectSingleNode("//body") ?? doc.DocumentNode;
        IReadOnlyList<Element> results = new[] { new Element(body) };
        foreach (var selector in cssSelectors)
        {
            if (string.IsNullOrWhiteSpace(selector)) continue;
            results = ApplyRule(results, selector);
            if (results.Count == 0) break;
        }
        return results;
    }
    private static IReadOnlyList<Element> ApplyRule(IReadOnlyList<Element> input, string selector)
    {
        var next = new List<Element>();
        foreach (var el in input)
        {
            try
            {
                foreach (var node in el.QueryNodes(selector))
                    next.Add(new Element(node));
            }
            catch
            {
            }
        }
        return next;
    }
}