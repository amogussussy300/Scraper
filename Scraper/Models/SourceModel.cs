using System.ComponentModel.DataAnnotations;
using Scraper.Core.ValidationAttributes;

namespace Scraper.Models
{
    public class SourceModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Link is required")]
        [ValidUrl(ErrorMessage = "Link must be a valid http(s) URL with a real domain")]
        public string Link { get; set; } = "";
    }
}
