using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StackExchange
{
    public class StackAuthSitesRequest : IAsyncOperationProvider<IEnumerable<StackAPI>>
    {
        protected HttpJsonRequest PendingRequest;

        #region IAsyncOperationProvider<IEnumerable<StackAPI>> Member

        public void Begin(Action<IEnumerable<StackAPI>> success, Action<Exception> error) {
            this.PendingRequest = new HttpJsonRequest(new Uri("http://stackauth.com/1.0/sites"));
            this.PendingRequest.Begin(response => {
                success(from i in response["api_sites"].Children()
                        select new StackAPI(
                            i["name"].Value<string>(),
                            new Uri(i["api_endpoint"].Value<string>()),
                            new Uri(i["site_url"].Value<string>()),
                            i["state"].Value<string>().ParseToEnum<APIState>()
                        ));
            }, error);
        }

        public bool IsRunning {
            get { return this.PendingRequest != null && this.PendingRequest.IsRunning; }
        }

        public void Abort() {
            if (this.PendingRequest != null) this.PendingRequest.Abort();
        }

        #endregion
    }
}
