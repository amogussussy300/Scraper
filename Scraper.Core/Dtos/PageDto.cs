namespace Scraper.Core.Dtos;

public class PageDto
{
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";                       // url page != source.Link
    public IList<ActionDto> Actions { get; set; } = new List<ActionDto>();
}