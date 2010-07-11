using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace StackExchange
{
    public abstract class APIRequest<TResponse> : APIObject, IAsyncOperationProvider<TResponse>
    {
        public APIRequest(StackAPI api)
            : base(api) { }

        protected abstract TResponse ProcessResponse(JObject response);

        protected HttpJsonRequest PendingRequest;

        #region IAsyncOperationProvider<TResponse> Member

        public abstract void Begin(Action<TResponse> success, Action<Exception> error);

        public bool IsRunning {
            get { return this.PendingRequest != null && this.PendingRequest.IsRunning; }
        }

        public void Abort() {
            if (this.PendingRequest != null) this.PendingRequest.Abort();
        }

        #endregion
    }
}
