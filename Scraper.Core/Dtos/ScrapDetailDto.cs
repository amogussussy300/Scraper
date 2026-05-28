using Scraper.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Core.Dtos
{
    public class ScrapDetailDto
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public ActionType ActionType { get; set; } = ActionType.ExtractField;

        public string Target { get; set; } = string.Empty;

        public List<ScrapActionDto> Actions { get; set; } = new();

    }
}
