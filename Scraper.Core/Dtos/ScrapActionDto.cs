using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Core.Dtos
{
    public class ScrapActionDto
    {
        public int Id { get; set; }
        public int ScrapDetailId { get; set; }
        public string Selector { get; set; } = "";
        public int Order { get; set; }
    }
}
