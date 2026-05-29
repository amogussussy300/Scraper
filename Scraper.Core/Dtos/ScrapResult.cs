using Scraper.Core.Enums;

namespace Scraper.Core.Dtos;

public class ScrapResult
{
    public List<string> Values { get; set; } = new();
    public List<string> MatchedHtml { get; set; } = new();
    public string RawHtml { get; set; } = "";
}


//public class TEMP_FIELDS {
//    public const String Title = "Title";
//}


//public class PageDto {
//    public String Name { get; set; }
//    public IList<ActionDto> Actions { get; set; } 
//}

//public class ActionDto { 
//    public ActionType ActionType { get; set; }
//    public IList<ExtractRuleDto> ExtractRules   { get; set; }
//}

//public class ExtractRuleDto { 
//    public ExtractRuleType Type { get; set; }
//    public String Value { get; set; }//jquerylike selector
//}

//public enum ExtractRuleType { 
//    CssSelector = 1
//}

//--playwrite service -> html
//--PageDto + html + parsingService  

// foreach(var action in page.Actions) parsingService.Extract(html, rules);

//    body = new Element(body)

//   var results = [body]

// foreach (var rule in rules){
//    results = results.apply(rule);
// }

//ApplyRule (selector){
// retunn elements
//}
