using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public class DataResponse<TItem>
    {
        public DataResponse(IEnumerable<TItem> items, int total, int page, int pageSize) {
            this.Items = items;
            this.Total = total;
            this.Page = page;
            this.PageSize = PageSize;
        }

        public IEnumerable<TItem> Items { get; private set; }
        public int Total { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }
    }
}
