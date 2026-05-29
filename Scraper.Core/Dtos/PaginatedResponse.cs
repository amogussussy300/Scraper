using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Core.Dtos
{
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class PagingParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool SortDescending { get; set; } = false;
        public string SortBy { get; set; }
    }
}
