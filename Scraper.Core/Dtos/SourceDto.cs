using System.ComponentModel.DataAnnotations;

namespace Scraper.Core.Dtos;

public class SourceDto
{
    public int? Id { get; set; }
    public string Name { get; set; } = "";
    public string Link { get; set; } = "";
}
