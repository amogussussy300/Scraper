using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
namespace Scraper.Parsing;

public class Element
{
    private readonly HtmlNode _node;
    public Element(HtmlNode node) => _node = node;
    public string InnerText => HtmlEntity.DeEntitize(_node.InnerText ?? "").Trim();
    public string OuterHtml => _node.OuterHtml;
    public string? Attr(string name) => _node.GetAttributeValue(name, null);
    internal IEnumerable<HtmlNode> QueryNodes(string selector) =>
        _node.QuerySelectorAll(selector) ?? Enumerable.Empty<HtmlNode>();
}