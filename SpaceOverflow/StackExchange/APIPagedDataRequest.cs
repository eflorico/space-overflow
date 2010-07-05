using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StackExchange
{
    public abstract class APIPagedDataRequest<TResponseItem> : APIRequest<DataResponse<TResponseItem>>
    {
        public APIPagedDataRequest(StackAPI api)
            : base(api) {
            this.Page = 1;
            this.PageSize = 30;
        }

       
        public int Page { get; set; }
        public int PageSize { get; set; }

        protected override Dictionary<string, string> Parameters {
            get {
                var parameters = new Dictionary<string, string>();
                parameters.Add("page", this.Page.ToString().ToLower());
                parameters.Add("pagesize", this.PageSize.ToString().ToLower());
                return parameters;
            }
        }

        protected abstract IEnumerable<TResponseItem> ProcessResponseItems(JObject response);

        protected override DataResponse<TResponseItem> ProcessResponse(JObject response) {
            return new DataResponse<TResponseItem>(this.ProcessResponseItems(response),
                response["total"].Value<int>(),
                response["page"].Value<int>(),
                response["pagesize"].Value<int>());
        }

        
    }
}
