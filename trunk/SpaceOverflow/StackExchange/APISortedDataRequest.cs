using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public abstract class APISortedDataRequest<TRepsonseItem, TSort> : APIPagedDataRequest<TRepsonseItem>
    {
        public APISortedDataRequest(StackAPI api)
            : base(api) {
            this.Order = Order.Descending;
        }

        public TSort Sort { get; set; }
        public Order Order { get; set; }

        protected override Dictionary<string, string> Parameters {
            get {
                var parameters = base.Parameters;
                parameters.Add("sort", this.Sort.ToString().ToLower());
                parameters.Add("order", this.Order == Order.Ascending ? "asc" : "desc");
                return parameters;
            }
        }
    }
}
