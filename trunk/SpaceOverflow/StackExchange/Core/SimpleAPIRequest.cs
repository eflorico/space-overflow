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
    public abstract class SimpleAPIRequest<TResponse> : APIRequest<TResponse>
    {
        public SimpleAPIRequest(StackAPI api)
            : base(api) { }

        protected abstract string Route { get; }
        protected abstract Dictionary<string, string> Parameters { get; }

        public Uri Uri {
            get {
                var parameters = this.Parameters;

                if (StackAPI.Key != null) parameters.Add("key", StackAPI.Key);

                var uri = this.API.BaseUri.ToString() + this.Route;
                uri += "?" + parameters.Aggregate("", (sum, item) => sum + Uri.EscapeDataString(item.Key) + "=" + Uri.EscapeDataString(item.Value) + "&");

                return new Uri(uri);
            }
        }

        public override void Begin(Action<TResponse> success, Action<Exception> error) {
            this.PendingRequest = new HttpJsonRequest(this.Uri);
            this.PendingRequest.Begin(new Action<JObject>(response => success(this.ProcessResponse(response))), error);
        }
    }
}