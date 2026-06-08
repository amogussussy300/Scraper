using Scraper.Core.Enums;
namespace Scraper.Core.Dtos;

public class ActionDto
{
    public ActionType ActionType { get; set; } = ActionType.ExtractField;
    public string FieldName { get; set; } = "";
    public IList<ExtractRuleDto> ExtractRules { get; set; } = new List<ExtractRuleDto>();
}