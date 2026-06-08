using Scraper.Core.Enums;
namespace Scraper.Core.Dtos;

public class ExtractRuleDto
{
    public ExtractRuleType Type { get; set; } = ExtractRuleType.CssSelector;
    public string Value { get; set; } = "";
}