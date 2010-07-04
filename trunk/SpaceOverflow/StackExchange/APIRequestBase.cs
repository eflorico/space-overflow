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
    public abstract class APIRequestBase : APIObject
    {
        public APIRequestBase(StackAPI api)
            : base(api) { }

        protected abstract string Route { get; }
        protected abstract Dictionary<string, string> Parameters { get; }

        protected Uri GetUri() {
            var parameters = this.Parameters;

            if (StackAPI.Key != null) parameters.Add("key", StackAPI.Key);

            var uri = this.API.APIBaseURI.ToString() + this.Route;
            uri += "?" + parameters.Aggregate("", (sum, item) => sum + Uri.EscapeDataString(item.Key) + "=" + Uri.EscapeDataString(item.Value) + "&");

            return new Uri(uri);
        }

        protected HttpWebRequest Request;

        protected HttpWebRequest CreateRequest() {
            var request = (HttpWebRequest)WebRequest.Create(this.GetUri());
            request.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
            request.Timeout = 10000;
            return request;
        }

        public void Abort() {
            if (this.Request != null) {
                this.Request.Abort();
                Debug.Print("-- Aborted request to " + this.Route);
            }
        }

        public bool IsLoading { get; protected set; }
    }
}
