using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StackExchange
{
    public abstract class APIDataRequest<TSort, TReponseItem> : APIRequest<DataResponse<TReponseItem>>
    {
        public APIDataRequest(StackAPI api)
            : base(api) {
            this.Order = Order.Descending;
            this.Page = 1;
            this.PageSize = 30;
        }

        public TSort Sort { get; set; }
        public Order Order { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        protected override Dictionary<string, string> Parameters {
            get {
                var parameters = new Dictionary<string, string>();
                parameters.Add("sort", this.Sort.ToString().ToLower());
                parameters.Add("order", this.Order == Order.Ascending ? "asc" : "desc");
                parameters.Add("page", this.Page.ToString().ToLower());
                parameters.Add("pagesize", this.PageSize.ToString().ToLower());
                return parameters;
            }
        }

        protected abstract IEnumerable<TReponseItem> ProcessResponseItems(JObject response);

        protected override DataResponse<TReponseItem> ProcessResponse(JObject response) {
            return new DataResponse<TReponseItem>(this.ProcessResponseItems(response),
                response["total"].Value<int>(),
                response["page"].Value<int>(),
                response["pagesize"].Value<int>());
        }

        
    }
}
