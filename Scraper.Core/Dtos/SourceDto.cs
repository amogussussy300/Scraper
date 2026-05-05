using System.ComponentModel.DataAnnotations;
using Scraper.Core.Validation;

namespace Scraper.Core.Dtos;

public class SourceDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Link is required")]
    [ValidUrl(ErrorMessage = "Link must be a valid http(s) URL with a real domain")]
    public string Link { get; set; } = "";
}
