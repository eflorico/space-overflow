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
    public abstract class APIRequest<TResponse> : APIRequestBase
    {
        public APIRequest(StackAPI api)
            : base(api) { }

        protected abstract TResponse ProcessResponse(JObject response);

        protected TResponse ReceiveResponse(HttpWebResponse response) {
            var responseStream = response.GetResponseStream();

            if (response.ContentEncoding.ToLower().Contains("gzip"))
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

            var reader = new StreamReader(responseStream);
            var json = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return this.ProcessResponse(JObject.Parse(json));
        }

        public TResponse GetResponse() {
            var request = this.CreateRequest();
            return this.ReceiveResponse((HttpWebResponse)request.GetResponse());
        }

        public void BeginGetResponse(Action<TResponse> callback) {
            this.Abort();

            Debug.Print("-- Request to " + this.Route + " started");

            this.IsLoading = true;
            this.Request = this.CreateRequest();
            this.Request.BeginGetResponse(new AsyncCallback(result => {
                try {
                    var httpResponse = (HttpWebResponse)this.Request.EndGetResponse(result);
                    Debug.Print("-- Got response for " + this.Route);
                    var response = this.ReceiveResponse(httpResponse);
                    callback(response);
                    this.Request = null;
                    Debug.Print("-- Response from " + this.Route + " processed");
                }
                catch (WebException) { }
                finally {
                    this.IsLoading = false;
                }
            }), null);
        }
    }
}